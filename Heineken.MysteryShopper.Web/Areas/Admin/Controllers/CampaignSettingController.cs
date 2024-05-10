using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SurveyUs.Application.Features.Campaign.Commands.Create;
using SurveyUs.Application.Features.Campaign.Commands.Update;
using SurveyUs.Application.Features.Campaign.Queries.GetAllCached;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Application.Features.Question.Queries.GetById;
using SurveyUs.Application.Features.QuestionCategory.Query.GetAllCached;
using SurveyUs.Application.Features.QuestionCategoryMapping.Commands.Create;
using SurveyUs.Application.Features.QuestionMappings.Commands.Create;
using SurveyUs.Application.Features.QuestionMappings.Commands.Update;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Domain.Enums;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CampaignSettingController : BaseController<CampaignSettingController>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionMappingsRepository _questionMappingsRepository;
        private readonly IConfiguration _config;

        private const string _campaignDataTable = "campaignTable";
        public CampaignSettingController(
            IQuestionRepository questionRepository,
            IQuestionMappingsRepository questionMappingsRepository,
            IConfiguration config
        )
        {
            _questionRepository = questionRepository;
            _questionMappingsRepository = questionMappingsRepository;
            _config = config;
        }

        public IActionResult Index(int storeId)
        {
            return View();
        }

        public async Task<IActionResult> LoadAll(int storeId)
        {
            var campaignResponses = await _mediator.Send(new GetAllCampaignsCachedQuery());
            if (campaignResponses.Succeeded)
            {
                var viewModel = _mapper.Map<List<CampaignSettingViewModel>>(campaignResponses.Data);
                
                foreach (var campaign in viewModel)
                {
                    var dateToday = DateTime.Now.Date;
                    if (dateToday >= campaign.StartDate && dateToday <= campaign.EndDate)
                    {
                        campaign.Status = StatusEnum.Active;
                    }
                    else if (dateToday >= campaign.StartDate && dateToday >= campaign.EndDate)
                    {
                        campaign.Status = StatusEnum.Expired;
                    }
                    else if (dateToday < campaign.StartDate && dateToday < campaign.EndDate)
                    {
                        campaign.Status = StatusEnum.Inactive;
                    }
                }
                return PartialView("_ViewAll",  viewModel);
            }
            return null;
        }
        public async Task<JsonResult> GetCampaignListing(DataTableExtensions request)
        {
            var con = _config.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));

            List<CampaignSettingViewModel> campaignList = new List<CampaignSettingViewModel>();

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

            #region Raw Query 
            var rawQuery = @"
                SELECT *,
                    CASE  
                       WHEN c.StartDate > CURRENT_TIMESTAMP and c.EndDate > CURRENT_TIMESTAMP THEN 'Inactive'
                       WHEN c.StartDate < CURRENT_TIMESTAMP and c.EndDate < CURRENT_TIMESTAMP THEN 'Expired'
                       WHEN c.StartDate <= CURRENT_TIMESTAMP and c.EndDate >= CURRENT_TIMESTAMP THEN 'Active'
                       ELSE null
                    END as ""Status""
                FROM dbo.Campaign c ";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"
                    WHERE LOWER(c.Name) LIKE '%' + @searchTerm + '%' ";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "name")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"c.Name ";
                }
                else
                    rawQuery += @"c.Name DESC ";
            }
            else if (sortColumnName == "startdatestring")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"c.StartDate ";
                }
                else
                    rawQuery += @"c.StartDate DESC ";
            }
            else if (sortColumnName == "enddatestring")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"c.EndDate ";
                }
                else
                    rawQuery += @"c.EndDate DESC ";
            }
            else if (sortColumnName == "statusstring")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += "\"Status\"";
                }
                else
                    rawQuery += "\"Status\" DESC ";
            }
            else
            {
                rawQuery += "c.CreatedOn DESC ";
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

                        var res = new CampaignSettingViewModel()
                        {
                            Id = dr["Id"] is DBNull ? 0 : Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"] is DBNull ? string.Empty : dr["Name"].ToString(),
                            StartDate = dr["StartDate"] is DBNull ? DateTime.MinValue : (DateTime)dr["StartDate"],
                            EndDate = dr["EndDate"] is DBNull ? DateTime.MinValue : (DateTime)dr["EndDate"],
                            StartDateString = dr["StartDate"] is DBNull ? "-" : ((DateTime)dr["StartDate"]).ToString("dd MMMM yyyy"),
                            EndDateString = dr["EndDate"] is DBNull ? "-" : ((DateTime)dr["EndDate"]).ToString("dd MMMM yyyy"),
                            StatusString = dr["Status"] is DBNull ? "-" : dr["Status"].ToString(),
                        };

                        campaignList.Add(res);
                    }
                }

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = campaignList
                });
            }
        }

        public async Task<IActionResult> OnGetCreateOrEdit(int id = 0)
        {
            if (id == 0)
            {
                var campaignViewModel = new CampaignSettingViewModel
                {
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    DataTableId = _campaignDataTable
                };

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", campaignViewModel) });
            }
            else
            {
                var campaign = await _mediator.Send(new GetCampaignByIdQuery() { Id = id });

                if (campaign.Succeeded)
                {
                    var campaignViewModel = _mapper.Map<CampaignSettingViewModel>(campaign.Data);
                    campaignViewModel.UpdatedOn = DateTime.Now;
                    campaignViewModel.DataTableId = _campaignDataTable;

                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", campaignViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(CampaignSettingViewModel userModel)
        {
            if (userModel.Id == 0)
            {
                var createCampaignCommand = _mapper.Map<CreateCampaignCommand>(userModel);

                var result = await _mediator.Send(createCampaignCommand);
                if (result.Succeeded)
                {
                    var name = result.Data;
                    _notify.Success($"Campaign {name} Created.");
                    return new JsonResult(new { isValid = true, dataTableId = _campaignDataTable });
                }
            }
            else 
            {
                var updateCampaignCommand = _mapper.Map<UpdateCampaignCommand>(userModel);
                var result = await _mediator.Send(updateCampaignCommand);
                if (result.Succeeded)
                {
                    var name = result.Data;
                    _notify.Information($"Campaign '{name}' Updated.");
                    return new JsonResult(new { isValid = true, dataTableId = _campaignDataTable });
                }
                _notify.Error($"Campaign '{userModel.Name}' Updated.");
            }

            return new JsonResult(new { isValid = false });
        }

        #region Unused
        //[HttpPost]
        //public async Task<IActionResult> OnPostDelete(int id)
        //{
        //    var deleteCommand = await _mediator.Send(new DeleteCampaignCommand() { Id = id });

        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"Campaign {deleteCommand.Data} Deleted.");

        //        var response = await _mediator.Send(new GetAllCampaignsCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<CampaignSettingViewModel>>(response.Data);
        //            var campaignsList = new List<CampaignSettingViewModel>();

        //            foreach (var model in viewModel)
        //            {
        //                var storesViewModel = new CampaignSettingViewModel()
        //                {
        //                    Id = model.Id,
        //                    Name = model.Name,
        //                    Description = model.Description,
        //                    StartDate = model.StartDate,
        //                    EndDate = model.EndDate,
        //                    CreatedOn = model.CreatedOn
        //                };
        //                campaignsList.Add(storesViewModel);
        //            }

        //            var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", campaignsList);
        //            return new JsonResult(new { isValid = true, html });
        //        }
        //        else
        //        {
        //            _notify.Error(response.Message);
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        _notify.Error(deleteCommand.Message);
        //        return null;
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> OnPostActivate(int id, bool isActivate)
        //{
        //    var query = await _mediator.Send(new GetCampaignByIdQuery() { Id = id });

        //    if (query.Succeeded) 
        //    {
        //        UpdateCampaignCommand updateCommand = _mapper.Map<UpdateCampaignCommand>(query.Data);
        //        updateCommand.IsActive = !isActivate;

        //        var result = await _mediator.Send(updateCommand);
        //        if (result.Succeeded)
        //        {
        //            string message;
        //            if (updateCommand.IsActive)
        //                message = "Active";
        //            else
        //                message = "Inactive";

        //            _notify.Information($"Campaign with ID {id} {message}.");
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            _notify.Error(result.Message);
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        _notify.Error(query.Message);
        //        return null;
        //    }
        //}
        #endregion

        public async Task<IActionResult> OnGetQuestionMapping(int campaignId)
        {
            var viewModel = new QuestionMappingViewModel();
            viewModel.CampaignId = campaignId;

            var questionCategories = await _mediator.Send(new GetAllQuestionCategoryCachedQuery());

            if (questionCategories.Succeeded)
                viewModel.QuestionCategoryList = questionCategories.Data.Select(x => new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }).ToList();

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_QuestionMapping", viewModel) });
        }

        public async Task<IActionResult> GetQuestionsByCategory(int categoryId)
        {
            var viewModel = _questionRepository.Questions.Where(x => x.CategoryId == categoryId).Select(x => new QuestionSelectViewModel()
            {
                QuestionId = x.Id,
                QuestionText = x.QuestionText
            }).ToList();

            return PartialView("_QuestionListing", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateQuestionMapping(QuestionMappingViewModel model)
        {
            var questionCategoriesList = new List<QuestionCategoryMapping>();

            foreach (var questionId in model.QuestionIds)
            {
                var question = await _mediator.Send(new GetQuestionByIdQuery() { Id = questionId });

                if (question.Succeeded)
                {
                    var existingMapping = _questionMappingsRepository.QuestionMappings
                        .FirstOrDefault(x => x.QuestionId == questionId && x.CampaignId == model.CampaignId);

                    var questionMapping = new QuestionMappings()
                    {
                        Id = (existingMapping != null) ? existingMapping.Id : 0,
                        QuestionId = (existingMapping != null) ? existingMapping.QuestionId : questionId,
                        CampaignId = (existingMapping != null) ? existingMapping.CampaignId : model.CampaignId
                    };

                    if (existingMapping != null)
                    {
                        var questionMappingCommand = _mapper.Map<UpdateQuestionMappingCommand>(questionMapping);
                        var result = await _mediator.Send(questionMappingCommand);

                        if (result.Succeeded)
                        {
                            if (questionCategoriesList.IsNullOrEmpty())
                            {
                                var questionCategoryMapping = new QuestionCategoryMapping()
                                {
                                    QuestionMappingId = result.Data,
                                    QuestionCategoryId = question.Data.CategoryId
                                };
                                questionCategoriesList.Add(questionCategoryMapping);
                            }

                            if (questionCategoriesList.Any(x => x.QuestionCategoryId != question.Data.CategoryId))
                            {
                                var questionCategoryMapping = new QuestionCategoryMapping()
                                {
                                    QuestionMappingId = result.Data,
                                    QuestionCategoryId = question.Data.CategoryId
                                };
                                questionCategoriesList.Add(questionCategoryMapping);
                            }
                        }
                    }
                    else
                    {
                        var questionMappingCommand = _mapper.Map<CreateQuestionMappingCommand>(questionMapping);
                        var result = await _mediator.Send(questionMappingCommand);

                        if (result.Succeeded)
                        {
                            if (questionCategoriesList.IsNullOrEmpty())
                            {
                                var questionCategoryMapping = new QuestionCategoryMapping()
                                {
                                    QuestionMappingId = result.Data,
                                    QuestionCategoryId = question.Data.CategoryId
                                };
                                questionCategoriesList.Add(questionCategoryMapping);
                            }

                            if (questionCategoriesList.Any(x => x.QuestionCategoryId != question.Data.CategoryId))
                            {
                                var questionCategoryMapping = new QuestionCategoryMapping()
                                {
                                    QuestionMappingId = result.Data,
                                    QuestionCategoryId = question.Data.CategoryId
                                };
                                questionCategoriesList.Add(questionCategoryMapping);
                            }
                        }
                    }

                }
            }

            foreach (var questionCategoryMapping in questionCategoriesList)
            {
                var questionMappingCategory = _mapper.Map<CreateQuestionCategoryMappingCommand>(questionCategoryMapping);
                await _mediator.Send(questionMappingCategory);
            }

            var response = await _mediator.Send(new GetAllCampaignsCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<CampaignSettingViewModel>>(response.Data);
                var campaignsList = new List<CampaignSettingViewModel>();

                foreach (var campaign in viewModel)
                {
                    var campaignDetail = new CampaignSettingViewModel()
                    {
                        Id = campaign.Id,
                        Name = campaign.Name,
                        StartDate = campaign.StartDate,
                        EndDate = campaign.EndDate,
                        CreatedOn = campaign.CreatedOn
                    };

                    var dateToday = DateTime.Now.Date;
                    if (dateToday >= campaign.StartDate && dateToday <= campaign.EndDate)
                    {
                        campaignDetail.Status = StatusEnum.Active;
                    }
                    else if (dateToday >= campaign.StartDate && dateToday >= campaign.EndDate)
                    {
                        campaignDetail.Status = StatusEnum.Expired;
                    }
                    else if (dateToday < campaign.StartDate && dateToday < campaign.EndDate)
                    {
                        campaignDetail.Status = StatusEnum.Inactive;
                    }

                    campaignsList.Add(campaignDetail);
                }

                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", campaignsList);
                return new JsonResult(new { isValid = true, html });
            }

            return default;
        }
    }
}
