using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SurveyUs.Application.Features.Question.Commands.Create;
using SurveyUs.Application.Features.Question.Commands.Update;
using SurveyUs.Application.Features.Question.Queries.GetById;
using SurveyUs.Application.Features.QuestionAnswers.Commands.Create;
using SurveyUs.Application.Features.QuestionAnswers.Commands.Update;
using SurveyUs.Application.Features.QuestionCategory.Commands.Create;
using SurveyUs.Application.Features.QuestionCategory.Commands.Update;
using SurveyUs.Application.Features.QuestionCategory.Query.GetAllCached;
using SurveyUs.Application.Features.QuestionCategory.Query.GetById;
using SurveyUs.Application.Features.QuestionChoices.Commands.Create;
using SurveyUs.Application.Features.QuestionChoices.Commands.Update;
using SurveyUs.Application.Features.QuestionChoices.Queries.GetById;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Extensions;
using QuestionAnswers = SurveyUs.Web.Areas.Admin.Models.QuestionAnswers;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionSettingController : BaseController<QuestionSettingController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IQuestionChoicesRepository _questionChoicesRepository;
        private readonly IQuestionAnswerRepository _questionAnswerRepository;
        private const string _categoryDataTable = "categoryTable";
        private const string _questionDataTable = "questionTable";

        public QuestionSettingController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IQuestionChoicesRepository questionChoicesRepository, IQuestionAnswerRepository questionAnswerRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _questionChoicesRepository = questionChoicesRepository;
            _questionAnswerRepository = questionAnswerRepository;
        }

        public IActionResult Index()
        {
            var viewModel = new QuestionViewModel();

            return View(viewModel);
        }

        public async Task<IActionResult> OnGetQuestionCategories(DataTableExtensions request)
        {
            var con = _configuration.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));

            var categoryList = new List<QuestionCategoryViewModel>();

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
                    SELECT qc.*, CONCAT(u.FirstName, ' ', u.LastName) as ""CreatorName""
                    FROM dbo.QuestionCategory qc
                    LEFT JOIN [Identity].Users u on u.Id = qc.CreatedBy ";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"
                    WHERE LOWER(qc.CategoryName) LIKE '%' + @searchTerm + '%'
                    OR LOWER(qc.CreatedBy) LIKE '%' + @searchTerm + '%' 
                    OR LOWER(CONCAT(u.FirstName, ' ', u.LastName)) LIKE '%' + @searchTerm + '%' ";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "categoryname")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"qc.CategoryName ";
                }
                else
                    rawQuery += @"qc.CategoryName DESC ";
            }
            else if (sortColumnName == "createdonstring")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"qc.CreatedOn ";
                }
                else
                    rawQuery += @"qc.CreatedOn DESC ";
            }
            else if (sortColumnName == "createdby")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"""CreatorName"" ";
                }
                else
                    rawQuery += @"""CreatorName"" DESC ";
            }
            else
            {
                rawQuery += "qc.CreatedOn DESC ";
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

                        var res = new QuestionCategoryViewModel()
                        {
                            Id = dr["Id"] is DBNull ? 0 : Convert.ToInt32(dr["Id"]),
                            CategoryName = dr["CategoryName"] is DBNull ? string.Empty : dr["CategoryName"].ToString(),
                            CreatedOnString = dr["CreatedOn"] is DBNull ? "-" : ((DateTime)dr["CreatedOn"]).ToString("dd MMMM yyyy"),
                            CreatedBy = dr["CreatorName"] is DBNull ? null : dr["CreatorName"].ToString(),
                        };

                        categoryList.Add(res);
                    }
                }

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = categoryList,
                });
            }
        }

        public async Task<IActionResult> OnGetQuestion(DataTableExtensions request)
        {
            var con = _configuration.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));
            var questionList = new List<QuestionViewModel>();

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
                    SELECT *
                    FROM (SELECT q.Id, qc.CategoryName, q.QuestionText, q.CreatedOn,
                        CASE
                            WHEN q.ResponseType = 1 THEN 'True False'
                            WHEN q.ResponseType = 2 THEN 'Multiple Choice'
                            WHEN q.ResponseType = 3 THEN 'Multiple Select'
                            WHEN q.ResponseType = 4 THEN 'Point Based'
                            WHEN q.ResponseType = 5 THEN 'Short Answer'
                            ELSE NULL
                        END AS ""ResponseType""
                    FROM dbo.Question q
                    LEFT JOIN dbo.QuestionCategory qc ON qc.Id = q.CategoryId) as subquery ";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"
                    WHERE LOWER(subquery.CategoryName) LIKE '%' + @searchTerm + '%'
                    OR LOWER(subquery.QuestionText) LIKE '%' + @searchTerm + '%'
                    OR LOWER(subquery.ResponseType) LIKE '%' + @searchTerm + '%'";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "categoryname")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"subquery.CategoryName ";
                }
                else
                    rawQuery += @"subquery.CategoryName DESC ";
            }
            else if (sortColumnName == "questiontext")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"subquery.QuestionText ";
                }
                else
                    rawQuery += @"subquery.QuestionText DESC ";
            }
            else if (sortColumnName == "responsetypestring")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"subquery.ResponseType ";
                }
                else
                    rawQuery += @"subquery.ResponseType DESC ";
            }
            else
            {
                rawQuery += "subquery.CreatedOn DESC ";
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
                            CategoryName = dr["CategoryName"] is DBNull ? string.Empty : dr["CategoryName"].ToString(),
                            QuestionText = dr["QuestionText"] is DBNull ? string.Empty : dr["QuestionText"].ToString(),
                            ResponseTypeString = dr["ResponseType"] is DBNull ? string.Empty : dr["ResponseType"].ToString(),
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

        public async Task<IActionResult> OnGetCreateOrEditQuestionCategory(int categoryId)
        {
            var questionCategoryViewModel = new QuestionCategoryViewModel
            {
                DataTableId = _categoryDataTable
            };

            if (categoryId > 0)
            {
                var categoryQuery = await _mediator.Send(new GetQuestionCategoryByIdQuery() { Id = categoryId });
                if (categoryQuery.Succeeded)
                {
                    var data = categoryQuery.Data;

                    questionCategoryViewModel = new QuestionCategoryViewModel()
                    {
                        DataTableId = _categoryDataTable,
                        Id = data.Id, 
                        CategoryName = data.CategoryName
                    };
                }
            }

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateQuestionCategory", questionCategoryViewModel) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEditQuestionCategory(QuestionCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                model.UpdatedBy = currentUser.Id;
                model.UpdatedOn = DateTime.Now;

                if (model.Id == 0)
                {
                    model.CreatedBy = currentUser.Id;
                    model.CreatedOn = DateTime.Now;
                    var createCategoryCommand = _mapper.Map<CreateQuestionCategoryCommand>(model);
                    var result = await _mediator.Send(createCategoryCommand);

                    if (result.Succeeded)
                    {
                        _notify.Success($"Question Category '{model.CategoryName}' Created.");
                        return new JsonResult(new { isValid = true, dataTableId = _categoryDataTable });
                    }
                    _notify.Error($"Failed To Create Question Category '{model.CategoryName}'.");
                }
                else
                {
                    var updateCategoryCommand = _mapper.Map<UpdateQuestionCategoryCommand>(model);

                    var result = await _mediator.Send(updateCategoryCommand);
                    if (result.Succeeded)
                    {
                        _notify.Success($"Question Category '{model.CategoryName}' Updated.");
                        return new JsonResult(new { isValid = true, dataTableId = _categoryDataTable });
                    }
                    _notify.Error($"Failed To Update Question Category '{model.CategoryName}'.");
                }

            }

            return new JsonResult(new { isValid = false });
        }

        public async Task<IActionResult> OnGetCreateOrEditQuestion(int questionId)
        {
            var viewModel = new QuestionViewModel();

            if (questionId > 0)
            {
                var questionQuery = await _mediator.Send(new GetQuestionByIdQuery() { Id = questionId });
                if (questionQuery.Succeeded)
                {
                    viewModel = _mapper.Map<QuestionViewModel>(questionQuery.Data);
                }

                var questionOptionQuery = await _mediator.Send(new GetQuestionChoicesByQuestionIdQuery() { QuestionId = questionId });

                if (questionOptionQuery.Succeeded && questionOptionQuery.Data.Count > 0)
                {
                    var choices = _mapper.Map<List<QuestionChoices>>(questionOptionQuery.Data);
                    viewModel.Options = choices.Select(c => c.ChoiceText).ToList();

                    foreach (var x in questionOptionQuery.Data)
                    {
                        var questionAnswers = _questionAnswerRepository.QuestionAnswers.Where(y => y.QuestionChoiceId == x.Id && !y.IsDeleted).ToList();

                        viewModel.QuestionOptions.Add(new QuestionAnswers
                        {
                            QuestionId = x.QuestionId,
                            QuestionChoiceId = x.Id,
                            ChoiceText = x.ChoiceText,
                            IsCorrect = questionAnswers.Any(y => y.QuestionChoiceId == x.Id)
                        });
                    }
                }
            }

            var questionTypes = EnumHelper.ToSelectionList<QuestionTypeEnum>();
            viewModel.QuestionTypeDropdown = questionTypes.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            var questionCategory = await _mediator.Send(new GetAllQuestionCategoryCachedQuery());
            if (questionCategory.Succeeded)
            {
                viewModel.QuestionCategoryDropdown = questionCategory.Data.Select(x => new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }).ToList();
            }

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateQuestion", viewModel) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEditQuestion(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                model.UpdatedBy = currentUser.Id;
                model.UpdatedOn = DateTime.Now;

                if (model.Id == 0)
                {
                    model.CreatedOn = DateTime.Now;
                    model.CreatedBy = currentUser.Id;

                    var createQuestionCommand = _mapper.Map<CreateQuestionCommand>(model);
                    var result = await _mediator.Send(createQuestionCommand);

                    if (result.Succeeded)
                    {
                        switch (result.Data.ResponseType)
                        {
                            case QuestionTypeEnum.MultipleChoice:
                            case QuestionTypeEnum.MultiSelect:
                                {
                                    foreach (var choice in model.Options)
                                    {
                                        var questionChoice = new QuestionChoices()
                                        {
                                            QuestionId = result.Data.Id,
                                            ChoiceText = choice
                                        };

                                        var questionChoiceCommand = _mapper.Map<CreateQuestionChoicesCommand>(questionChoice);
                                        await _mediator.Send(questionChoiceCommand);
                                    }
                                    break;
                                }
                            case QuestionTypeEnum.TrueFalse:
                                {
                                    var questionChoiceList = new List<QuestionChoices>
                                    {
                                        new ()
                                        {
                                            QuestionId = result.Data.Id,
                                            ChoiceText = "True"
                                        },
                                        new ()
                                        {
                                            QuestionId = result.Data.Id,
                                            ChoiceText = "False"
                                        }
                                    };

                                    var questionChoiceCommand = _mapper.Map<List<CreateQuestionChoicesCommand>>(questionChoiceList);

                                    foreach (var questionChoice in questionChoiceCommand)
                                    {
                                        await _mediator.Send(questionChoice);
                                    }
                                    break;
                                }
                        }

                        _notify.Success($"Question is Created.");
                        return new JsonResult(new { isValid = true, dataTableId = _questionDataTable });
                    }
                }
                else
                {
                    UpdateQuestionCommand updateQuestionCommand = new UpdateQuestionCommand
                    {
                        Id = model.Id,
                        CategoryId = model.CategoryId,
                        QuestionText = model.QuestionText,
                        Points = model.Points, 
                        UpdatedOn = model.UpdatedOn,
                        UpdatedBy = model.UpdatedBy,
                        HasMedia = model.HasMedia,
                        HasReward = model.HasReward,
                        DisplayOrder = model.DisplayOrder
                    };

                    var result = await _mediator.Send(updateQuestionCommand);

                    if (result.Succeeded && (result.Data.ResponseType == QuestionTypeEnum.MultipleChoice || result.Data.ResponseType == QuestionTypeEnum.MultiSelect))
                    {
                        //var deleteRequest = await _questionChoicesRepository.DeleteByQuestionIdAsync(updateQuestionCommand.Id);

                        var questionOptionQuery = await _mediator.Send(new GetQuestionChoicesByQuestionIdQuery() { QuestionId = updateQuestionCommand.Id });

                        if (questionOptionQuery.Succeeded)
                        {
                            var questionChoices = questionOptionQuery.Data;
                            var removeChoices = questionChoices.Where(x => !model.Options.Contains(x.ChoiceText)).ToList();

                            foreach (var choice in removeChoices)
                            {
                                var updateQuestionChoices = new UpdateQuestionChoicesCommand
                                {
                                    Id = choice.Id,
                                    QuestionId = choice.QuestionId,
                                    ChoiceText = choice.ChoiceText,
                                    LastModifiedBy = currentUser.Id,
                                    LastModifiedOn = DateTime.Now,
                                    IsDeleted = true
                                };
                                await _mediator.Send(updateQuestionChoices);
                            }

                            for (int i = 0; i < questionChoices.Count(x => model.Options.Any(y => y == x.ChoiceText)); i++)
                            {
                                var updateQuestionChoices = new UpdateQuestionChoicesCommand
                                {
                                    Id = questionChoices[i].Id,
                                    QuestionId = questionChoices[i].QuestionId,
                                    ChoiceText = model.Options[i],
                                    LastModifiedBy = currentUser.Id,
                                    LastModifiedOn = DateTime.Now,
                                };

                                await _mediator.Send(updateQuestionChoices);
                            }
                        }
                    }

                    var questionOptions = model.QuestionOptions;

                    foreach (var questionAnswer in questionOptions)
                    {
                        var existingQuestionAnswers = _questionAnswerRepository.QuestionAnswers
                            .FirstOrDefault(x =>
                                x.QuestionId == questionAnswer.QuestionId &&
                                x.QuestionChoiceId == questionAnswer.QuestionChoiceId);

                        if (existingQuestionAnswers == null && questionAnswer.IsCorrect)
                        {
                            var questionAnswerCommand = new CreateQuestionAnswerCommand
                            {
                                QuestionId = questionAnswer.QuestionId,
                                QuestionChoiceId = questionAnswer.QuestionChoiceId,
                                CreatedBy = currentUser.Id,
                                CreatedOn = DateTime.Now
                            };

                            await _mediator.Send(questionAnswerCommand);
                        }
                        else if (existingQuestionAnswers != null && !questionAnswer.IsCorrect)
                        {
                            var questionAnswerCommand = new UpdateQuestionAnswersCommand()
                            {
                                Id = existingQuestionAnswers.Id,
                                IsDeleted = true,
                                LastModifiedBy = currentUser.Id,
                                LastModifiedOn = DateTime.Now
                            };

                            await _mediator.Send(questionAnswerCommand);
                        }
                        else if (existingQuestionAnswers != null && questionAnswer.IsCorrect)
                        {
                            var questionAnswerCommand = new UpdateQuestionAnswersCommand()
                            {
                                Id = existingQuestionAnswers.Id,
                                IsDeleted = false,
                                LastModifiedBy = currentUser.Id,
                                LastModifiedOn = DateTime.Now
                            };

                            await _mediator.Send(questionAnswerCommand);
                        }
                    }

                    _notify.Success($"Question is Updated.");
                    return new JsonResult(new { isValid = true, dataTableId = _questionDataTable });
                }
            }

            _notify.Error($"Failed to Create/Update Question.");
            return new JsonResult(new { isValid = false });
        }
    }
}
