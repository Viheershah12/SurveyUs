using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Application.Features.QuestionMappings.Commands.Create;
using SurveyUs.Application.Features.QuestionMappings.Commands.Delete;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionMappingController : BaseController<QuestionMappingController>
    {
        private readonly IConfiguration _config;
        private readonly IQuestionMappingsRepository _questionMappingsRepository;

        private const string _questionDataTable = "questionTable";

        public QuestionMappingController(
            IConfiguration config, 
            IQuestionMappingsRepository questionMappingsRepository)
        {
            _config = config;
            _questionMappingsRepository = questionMappingsRepository;
        }

        public async Task<IActionResult> Index(int campaignId)
        {
            var campaignQuery = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });

            var viewModel = new QuestionMappingViewModel
            {
                CampaignId = campaignId,
                CampaignName = campaignQuery.Data.Name
            };

            return View(viewModel);
        }

        public async Task<IActionResult> GetQuestionMappingListing(QuestionMappingDataTable request)
        {
            var con = _config.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));

            List<QuestionViewModel> questionList = new List<QuestionViewModel>();

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
            //paramName.Add("@categoryId");
            //paramValue.Add(request.CategoryId);

            #region Raw Query 
            var rawQuery = @"
                SELECT q.Id, qc.CategoryName, q.QuestionText, qm.CampaignId
                FROM dbo.QuestionCategory qc
                LEFT JOIN dbo.Question q on q.CategoryId = qc.Id
                LEFT JOIN dbo.QuestionMappings qm on qm.QuestionId = q.Id and qm.CampaignId = @campaignId ";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"WHERE LOWER(q.QuestionText) LIKE '%' + @searchTerm + '%'
                              OR LOWER(qc.CategoryName) LIKE '%' + @searchTerm + '%' ";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "questiontext")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"q.QuestionText ";
                }
                else
                    rawQuery += @"q.QuestionText DESC ";
            }
            else if (sortColumnName == "categoryname")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"qc.CategoryName ";
                }
                else
                    rawQuery += @"qc.CategoryName DESC ";
            }
            else
            {
                rawQuery += @"q.CreatedOn DESC ";
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

                        var res = new QuestionViewModel()
                        {
                            Id = dr["Id"] is DBNull ? 0 : Convert.ToInt32(dr["Id"]),
                            QuestionText = dr["QuestionText"] is DBNull ? null : Convert.ToString(dr["QuestionText"]),
                            CategoryName = dr["CategoryName"] is DBNull ? null : Convert.ToString(dr["CategoryName"]),
                            IsAssigned = dr["CampaignId"] is DBNull ? false : Convert.ToInt32(dr["CampaignId"]) == request.CampaignId
                        };
                        questionList.Add(res);
                    }
                }

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = questionList,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAssign(List<int> editedQuestionIds, int campaignId)
        {
            var questionMappings = await _questionMappingsRepository.QuestionMappings.Where(c => c.CampaignId == campaignId).ToListAsync();

            var questionMappingRows = questionMappings.Where(c => editedQuestionIds.Contains(c.QuestionId)).ToList();

            var questionMappingStoreIds = questionMappingRows.Select(mapping => mapping.QuestionId).Distinct().ToList();
            var questionIdsNotInQuestionMappings = editedQuestionIds.Except(questionMappings.Select(mapping => mapping.QuestionId)).ToList();
            Result<int> result = new Result<int>();
            Result<int> deleteCommand = new Result<int>();

            try
            {
                foreach (var questionMapping in questionMappingRows)
                {
                    deleteCommand = await _mediator.Send(new DeleteQuestionMappingsCommand { Id = questionMapping.Id });

                }

                foreach (var questionId in questionIdsNotInQuestionMappings)
                {
                    QuestionMappings newCampaignMapping = new QuestionMappings()
                    {
                        CampaignId = campaignId,
                        QuestionId = questionId,
                    };

                    var createCampaignMappingsCommand = _mapper.Map<CreateQuestionMappingCommand>(newCampaignMapping);
                    result = await _mediator.Send(createCampaignMappingsCommand);
                }
                _notify.Information($"Question Mappings Updated Successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
