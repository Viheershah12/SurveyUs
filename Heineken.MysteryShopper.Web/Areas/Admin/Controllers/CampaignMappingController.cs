using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Application.Features.CampaignMappings.Commands.Create;
using SurveyUs.Application.Features.CampaignMappings.Commands.Delete;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CampaignMappingController : BaseController<CampaignSettingController>
    {
        private const int _maxlimt = 10;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStoreRepository _storeRepository;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;
        private readonly IConfiguration _configuration;

        public CampaignMappingController(UserManager<ApplicationUser> userManager,
            IStoreRepository storeRepository,
            ICampaignMappingsRepository campaignMappingsRepository,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _storeRepository = storeRepository;
            _campaignMappingsRepository = campaignMappingsRepository;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(int campaignId)
        {
            var campaign = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });

            var viewModel = new StoresMappingViewModel
            {
                CampaignId = campaignId,
                CampaignName = campaign.Succeeded ? campaign.Data.Name : string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> GetCampaignMappingsListing(CampaignMappingsDataTable request)
        {
            var con = _configuration.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));

            var storeList = new List<StoreViewModel>();

            var skipCount = (pageNumber - 1) * request.length;

            var searchValue = request.search.value?.ToLower().Trim();
            var sortColumn = request.order[0]?.column;
            string sortColumnDirection = request.order[0]?.dir;
            string? sortColumnName = null;

            if (sortColumn != null && !string.IsNullOrEmpty(sortColumnDirection))
            {
                sortColumnName = request.columns[(int)sortColumn].name.ToLower();
            }

            var paramName = new List<string>();
            var paramValue = new List<object>();

            paramName.Add("@campaignId");
            paramValue.Add(request.CampaignId);

            #region Raw Query 
            var rawQuery = @"
                    SELECT s.Id as ""StoreId"", s.Name, s.State, s.CreatedOn, cm.CampaignId, sta.Name
                    FROM dbo.Store s
                    LEFT JOIN dbo.CampaignMappings cm on s.Id = cm.StoreId and cm.CampaignId = @campaignId
                    LEFT JOIN dbo.State sta on sta.Id = s.State ";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"
                    WHERE LOWER(s.Name) LIKE '%' + @searchTerm + '%' 
                    OR LOWER(sta.Name) LIKE '%' + @searchTerm + '%' ";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "name")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"s.Name ";
                }
                else
                    rawQuery += @"s.Name DESC ";
            }
            else if (sortColumnName == "state")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"sta.Name ";
                }
                else
                    rawQuery += @"sta.Name DESC ";
            }
            else
            {
                rawQuery += "s.Id ";
            }
            #endregion

            var selectQuery = new StringBuilder();
            selectQuery.Append(rawQuery);

            // create connection object first
            using (var cn = new SqlConnection(con))
            {
                var (totalCount, dt) = await QueryRepositoryExtensions.SelectDataTableWithCountAndPagination(cn, rawQuery.ToString(), paramName.ToArray(), paramValue.ToArray(), skipCount, request.length);

                using (dt)
                {
                    for (int i = 0; i < dt?.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];

                        if (dr == null)
                            continue;

                        var res = new StoreViewModel()
                        {
                            Id = dr["StoreId"] is DBNull ? 0 : Convert.ToInt32(dr["StoreId"]),
                            Name = dr["Name"] is DBNull ? string.Empty : dr["Name"].ToString(),
                            State = dr["State"] is DBNull ? 0 : (StateEnum)dr["State"],
                            CreatedOn = dr["CreatedOn"] is DBNull ? DateTime.Now : (DateTime)dr["CreatedOn"],
                            IsAssigned = dr["CampaignId"] is DBNull ? false : Convert.ToInt32(dr["CampaignId"]) == request.CampaignId
                        };

                        storeList.Add(res);
                    }
                }

                return Json(new { 
                    draw = request.draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = storeList,
                    campaignId = request.CampaignId
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAssign(List<int> editedStoreIds, int campaignId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var campaignMappings = await _campaignMappingsRepository.CampaignMappings.Where(c => c.CampaignId == campaignId).ToListAsync();

            var campaignMappingRows = campaignMappings.Where(c => editedStoreIds.Contains(c.StoreId)).ToList();

            var campaignMappingStoreIds = campaignMappingRows.Select(mapping => mapping.StoreId).Distinct().ToList();
            var storeIdsNotInCampaignMappings = editedStoreIds.Except(campaignMappings.Select(mapping => mapping.StoreId)).ToList();
            Result<int> result = new Result<int>();
            Result<int> deleteCommand = new Result<int>();

            try
            {
                foreach (var campaignMapping in campaignMappingRows)
                {
                    deleteCommand = await _mediator.Send(new DeleteCampaignMappingsCommand { Id = campaignMapping.Id });

                }
                foreach (var storeId in storeIdsNotInCampaignMappings)
                {
                    CampaignMappingsViewModel newCampaignMapping = new CampaignMappingsViewModel()
                    {
                        StoreId = storeId,
                        CampaignId = campaignId,
                        IsDeleted = false,
                        CreatedBy = currentUser.Id,
                        UpdatedBy = currentUser.Id
                    };

                    var createCampaignMappingsCommand = _mapper.Map<CreateCampaignMappingsCommand>(newCampaignMapping);
                    result = await _mediator.Send(createCampaignMappingsCommand);
                }
                _notify.Information($"Campaign Mapping Updated Successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region OldCode
        //public async Task<IActionResult> LoadAll(int campaignId)
        //{
        //    //var storesList = await GetCampaignMappingsListing(campaignId, 1);

        //    var storesList = await GetCampaignStores(campaignId);
        //    return PartialView("_ViewAll", storesList);
        //}

        //public async Task<StoresMappingViewModel> GetCampaignStores(int campaignId)
        //{
        //    var campaign = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });
        //    var storesList = new StoresMappingViewModel();
        //    var campaignMappingsStores = await _campaignMappingsRepository.CampaignMappings.Where(c => c.CampaignId == campaignId && c.IsDeleted == false).ToListAsync();
        //    var campaignMappingsStoreIds = campaignMappingsStores.Select(c => c.StoreId).ToList();

        //    var stores = _storeRepository.Store.Where(x => x.IsActive).ToList();

        //    var storeOptionsList = stores.Select(x => new StoreViewModel()
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        State = x.State,
        //        CreatedOn = x.CreatedOn,
        //        IsAssigned = campaignMappingsStoreIds.Any(y => y == x.Id)
        //    }).ToList();

        //    storesList = new StoresMappingViewModel
        //    {
        //        CampaignId = campaignId,
        //        CampaignName = campaign.Succeeded ? campaign.Data.Name : string.Empty,
        //        Stores = storeOptionsList
        //    };
        //    return storesList;
        //}
        #endregion
    }
}
