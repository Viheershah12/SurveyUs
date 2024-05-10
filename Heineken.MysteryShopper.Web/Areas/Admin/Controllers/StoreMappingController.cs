using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyUs.Application.Features.Store.Queries.GetById;
using SurveyUs.Application.Features.StoreMappings.Commands.Create;
using SurveyUs.Application.Features.StoreMappings.Commands.Delete;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreMappingController : BaseController<StoreMappingController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStoreMappingsRepository _storeMappingsRepository;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;
        private readonly IConfiguration _config;

        public StoreMappingController(UserManager<ApplicationUser> userManager,
            IStoreMappingsRepository storeMappingsRepository,
            ICampaignMappingsRepository campaignMappingsRepository,
            IConfiguration config
        )
        {
            _userManager = userManager;
            _storeMappingsRepository = storeMappingsRepository;
            _campaignMappingsRepository = campaignMappingsRepository;
            _config = config;
        }

        public async Task<IActionResult> Index(int storeId)
        {
            var viewModel = new ShopperMappingViewModel();

            try
            {
                var store = await _mediator.Send(new GetStoreByIdQuery() { Id = storeId });

                viewModel = new ShopperMappingViewModel
                {
                    StoreId = storeId,
                    StoreName = store.Succeeded ? store.Data.Name : string.Empty
                };
            }
            catch (Exception ex)
            {

            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> GetMysteryShopperListing(MysteryShopperDataTable request)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var con = _config.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));

            List<UserViewModel> userList = new List<UserViewModel>();

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

            paramName.Add("@storeId");
            paramValue.Add(request.StoreId);

            #region Raw Query 
            var rawQuery = @"
                    SELECT u.Id, u.FirstName, u.LastName, u.Email, sm.StoreId
                    FROM [Identity].Users u
                    LEFT JOIN [Identity].UserRoles ur on ur.UserId = u.Id
                    left join [Identity].Roles r on r.Id = ur.RoleId
                    LEFT JOIN [dbo].StoreMappings sm on sm.UserId = ur.UserId and sm.StoreId = @storeId
                    WHERE r.NormalizedName = 'MYSTERY DRINKER' ";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"
                    AND LOWER(u.FirstName) LIKE '%' + @searchTerm + '%' 
                    OR LOWER(u.LastName) LIKE '%' + @searchTerm + '%'
                    OR LOWER(u.Email) LIKE '%' + @searchTerm + '%'
                    OR CONCAT(u.FirstName, ' ', u.LastName) LIKE '%' + @searchTerm + '%' ";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "firstname")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"u.FirstName ";
                }
                else
                    rawQuery += @"u.FirstName DESC ";
            }
            else if (sortColumnName == "email")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"u.Email ";
                }
                else
                    rawQuery += @"u.Email DESC ";
            }
            else
            {
                rawQuery += "u.CreatedOn DESC ";
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

                        var res = new UserViewModel()
                        {
                            Id = dr["Id"] is DBNull ? null : Convert.ToString(dr["Id"]),
                            FirstName = dr["FirstName"] is DBNull ? null : Convert.ToString(dr["FirstName"]),
                            LastName = dr["LastName"] is DBNull ? null : Convert.ToString(dr["LastName"]),
                            Email = dr["Email"] is DBNull ? null : Convert.ToString(dr["Email"]),
                            IsAssigned = dr["StoreId"] is DBNull ? false : Convert.ToInt32(dr["StoreId"]) == request.StoreId
                        };
                        userList.Add(res);
                    }
                }

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = userList,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAssign(List<string> editedUserIds, int storeId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                var storeMappings = await _storeMappingsRepository.StoreMappings.Where(s => s.StoreId == storeId).ToListAsync();

                var storeMappingRows = storeMappings.Where(s => editedUserIds.Contains(s.UserId)).ToList();

                var storeMappingUserIds = storeMappingRows.Select(mapping => mapping.UserId).Distinct().ToList();
                var userIdsNotInStoreMappings = editedUserIds.Except(storeMappings.Select(mapping => mapping.UserId)).ToList();

                foreach (var storeMapping in storeMappingRows)
                {
                    var deleteCommand = await _mediator.Send(new DeleteStoreMappingsCommand { Id = storeMapping.Id });
                }
                foreach (var userId in userIdsNotInStoreMappings)
                {
                    StoreMappingsViewModel newStoreMapping = new StoreMappingsViewModel()
                    {
                        UserId = userId,
                        StoreId = storeId,
                        IsDeleted = false,
                        CreatedBy = currentUser.Id,
                        UpdatedBy = currentUser.Id
                    };

                    var createStoreMappingsCommand = _mapper.Map<CreateStoreMappingsCommand>(newStoreMapping);
                    var result = await _mediator.Send(createStoreMappingsCommand);
                }
            }
            catch (Exception ex)
            {
                _notify.Information($"Failed To Update Mystery Drinker Mapping.");
                return BadRequest(ex.Message);
            }

            _notify.Information($"Mystery Drinker Mapping Updated Successfully.");
            return Ok();
        }

        #region Unused
        //public async Task<IActionResult> LoadAll(int storeId)
        //{
        //    var usersList = await GetMysterShoppers(storeId);
        //    return PartialView("_ViewAll", usersList);
        //}

        //public async Task<ShopperMappingViewModel> GetMysterShoppers(int storeId)
        //{
        //    var store = await _mediator.Send(new GetStoreByIdQuery() { Id = storeId });

        //    var usersList = new ShopperMappingViewModel();

        //    try
        //    {
        //        var storeMappingsUsers = await _storeMappingsRepository.StoreMappings.Where(u => u.StoreId == storeId && u.IsDeleted == false).ToListAsync();
        //        var storeMappingsUserIds = storeMappingsUsers.Select(u => u.UserId).ToList();

        //        var mysteryShopperUsers = (await _userManager.GetUsersInRoleAsync("Mystery Shopper")).Where(x => x.IsActive).ToList();

        //        var mysteryShoppersList = mysteryShopperUsers.Select(x => new UserViewModel()
        //        {
        //            Id = x.Id,
        //            FirstName = x.FirstName?.ToUpper(),
        //            LastName = x.LastName?.ToUpper(),
        //            Email = x.Email,
        //            IsAssigned = storeMappingsUserIds.Any(y => y == x.Id)
        //        }).ToList();

        //        usersList = new ShopperMappingViewModel
        //        {
        //            StoreId = storeId,
        //            StoreName = store.Succeeded ? store.Data.Name : string.Empty,
        //            MShopperMapping = mysteryShoppersList
        //        };
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return usersList;
        //}
        #endregion


    }
}
