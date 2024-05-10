using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Application.Features.QuestionMedia.Queries.GetById;
using SurveyUs.Application.Features.Store.Queries.GetById;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Areas.Questions.Model;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserSubmissionController : BaseController<UserSubmissionController>
    {
        private readonly IUserAnswersRepository _userAnswersRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IQuestionMappingsRepository _questionMappingsRepository;
        private readonly IQuestionChoicesRepository _questionChoicesRepository;

        public UserSubmissionController(
            IUserAnswersRepository userAnswersRepository, 
            IStoreRepository storeRepository, 
            IQuestionRepository questionRepository, 
            UserManager<ApplicationUser> userManager,
            IQuestionMappingsRepository questionMappingsRepository,
            IQuestionChoicesRepository questionChoicesRepository)
        {
            _userAnswersRepository = userAnswersRepository;
            _userManager = userManager;
            _questionRepository = questionRepository;
            _storeRepository = storeRepository;
            _questionMappingsRepository = questionMappingsRepository;
            _questionChoicesRepository = questionChoicesRepository;
        }

        public async Task<IActionResult> Index(int campaignId)
        {
            var campaignDetail = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });

            if (campaignDetail.Succeeded)
            {
                var viewModel = new UserSubmissionViewModel
                {
                    CampaignId = campaignId,
                    CampaignName = campaignDetail.Data.Name
                };

                return View(viewModel);
            }
            return RedirectToAction("Index", "CampaignSetting");
        }

        public async Task<IActionResult> LoadAll(int campaignId)
        {
            var viewModel = new UserSubmissionViewModel
            {
                CampaignId = campaignId,
            };

            var submissionList = new List<UserSubmission>();

            //load all unique user-store for that campaign
            var userStore = _userAnswersRepository.UserAnswers.Where(x => x.CampaignId == campaignId).ToList();
            var userStoreUnique = userStore.GroupBy(x => new { x.StoreId, x.UserId }).Select(group => group.First()).ToList();

            if (userStore.Count > 0)
            {
                var storeIds = userStore.Select(x => x.StoreId).ToList();
                var storeDetails = _storeRepository.Store.Where(s => storeIds.Contains(s.Id)).ToList();

                var userIds = userStore.Select(x => x.UserId).ToList();
                var mysteryShopperDetails = (await _userManager.GetUsersInRoleAsync("Mystery Drinker")).Where(x => userIds.Contains(x.Id)).ToList();

                foreach (var item in userStoreUnique)
                {
                    var store = storeDetails.Where(s => s.Id == item.StoreId).FirstOrDefault();

                    var mysteryShopper = mysteryShopperDetails.Where(ms => ms.Id == item.UserId).FirstOrDefault();

                    var model = new UserSubmission
                    {
                        StoreId = store.Id,
                        StoreName = store.Name,
                        UserId = item.UserId,
                        UserName = mysteryShopper?.FirstName + " " + mysteryShopper?.LastName,
                    };
                    submissionList.Add(model);
                }
            }

            viewModel.UserSubmissions = submissionList;
            return PartialView("_ViewAll", viewModel);
        }

        public async Task<IActionResult> ViewSubmission(int storeId, int campaignId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var storeDetail = await _mediator.Send(new GetStoreByIdQuery() { Id = storeId });
            var campaignDetail = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });
            var submissionTime = _userAnswersRepository.UserAnswers
                .Where(ans => ans.StoreId == storeId && ans.CampaignId == campaignId && ans.UserId == userId)
                .OrderByDescending(ans => ans.CreatedOn).Select(ans => ans.CreatedOn).FirstOrDefault();

            var viewModel = new QuestionsViewModel()
            {
                StoreId = storeId,
                StoreName = storeDetail.Data.Name,
                CampaignId = campaignId,
                CampaignName = campaignDetail.Data.Name,
                UserId = userId,
                UserName = user.UserName,
                SubmissionTime = submissionTime
            };

            return View("QuestionIndex", viewModel);
        }

        public async Task<IActionResult> SwitchPage(int storeId, int campaignId, string userId, int pageNumber = 1, int pageSize = 5)
        {
            var validFilter = new QuestionPaginationFilter(pageNumber, pageSize);
            var skipCount = (validFilter.PageNumber - 1) * validFilter.PageSize;

            var campaignQuery = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });
            var dateToday = DateTime.Now.Date;
            bool isActive = true;

            if (campaignQuery.Succeeded && campaignQuery.Data != null)
            {
                var campaignDetail = _mapper.Map<CampaignSettingViewModel>(campaignQuery.Data);
                if (dateToday >= campaignDetail.StartDate && dateToday <= campaignDetail.EndDate)
                {
                    campaignDetail.Status = StatusEnum.Active;
                }
                else if (dateToday >= campaignDetail.StartDate && dateToday >= campaignDetail.EndDate)
                {
                    campaignDetail.Status = StatusEnum.Expired;
                    isActive = false;
                }
                else if (dateToday < campaignDetail.StartDate && dateToday < campaignDetail.EndDate)
                {
                    campaignDetail.Status = StatusEnum.Inactive;
                    isActive = false;
                }
            }

            var questionMapping = _questionMappingsRepository.QuestionMappings.Include(x => x.Question).Where(x => x.CampaignId == campaignId).OrderBy(x => x.Question.DisplayOrder).Skip(skipCount).Take(validFilter.PageSize).ToList();
            var totalQuestions = _questionMappingsRepository.QuestionMappings.Where(x => x.CampaignId == campaignId).Count();

            //var questionMappings = _questionMappingsRepository.GetByCampaignIdAsync(campaignId);
            var questionDictionary = new Dictionary<Question, List<QuestionChoices>>();
            var questionMedia = new List<QuestionMediaFile>();

            foreach (var mapping in questionMapping)
            {
                var question = _questionRepository.Questions.FirstOrDefault(x => x.Id == mapping.QuestionId);

                if (question != null)
                {
                    var questionChoices = _questionChoicesRepository.QuestionChoices.Where(x => x.QuestionId == question.Id).ToList();
                    questionDictionary.Add(question, questionChoices);

                    var questionMediaResponse = await _mediator.Send(new GetQuestionMediaByQuestionIdQuery
                    {
                        QuestionId = question.Id,
                        CampaignId = campaignId,
                        StoreId = storeId,
                        UserId = userId
                    });

                    if (questionMediaResponse.Succeeded)
                    {
                        questionMedia.AddRange(
                        questionMediaResponse.Data.Select(x => new QuestionMediaFile
                        {
                            Id = x.Id,
                            QuestionId = x.QuestionId,
                            ContentType = x.ContentType,
                            Extension = x.Extension,
                            Name = x.Name,
                            FileBinary = x.FileBinary
                        }).ToList()
                        );
                    }
                }
            }

            var questionsModel = questionDictionary.Select(x => new QuestionsModel()
            {
                QuestionId = x.Key.Id,
                ResponseType = x.Key.ResponseType,
                QuestionText = x.Key.QuestionText,
                Points = x.Key.Points,
                AnswerOptions = x.Value.Select(y => new AnswerOption()
                {
                    OptionId = y.Id,
                    OptionText = y.ChoiceText
                }).ToList(),
                HasMedia = x.Key.HasMedia,
                QuestionMediaList = questionMedia.Where(q => q.QuestionId == x.Key.Id).ToList(),
                MultiSelectOptions = x.Value.Select(x => new MultiSelect
                {
                    OptionId = x.Id,
                    OptionText = x.ChoiceText,
                    IsSelected = false
                }).ToList()
            }).ToList();

            var completedQuizAnswers = _userAnswersRepository.UserAnswers.Where(a => a.CampaignId == campaignId && a.StoreId == storeId && a.UserId == userId).ToList();

            if (completedQuizAnswers.Count != 0)
            {
                foreach (var question in questionsModel)
                {
                    var answer = new UserAnswers();
                    var multiAnswerList = new List<UserAnswers>();

                    if (question.ResponseType != QuestionTypeEnum.MultiSelect)
                        answer = completedQuizAnswers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                    else
                        multiAnswerList.AddRange(completedQuizAnswers.Where(a => a.QuestionId == question.QuestionId).ToList());

                    if (answer != null)
                    {
                        switch (question.ResponseType)
                        {
                            case QuestionTypeEnum.TrueFalse:
                            case QuestionTypeEnum.MultipleChoice:
                                {
                                    question.QuestionChoiceId = answer.QuestionChoiceId ?? 0;
                                    break;
                                }
                            case QuestionTypeEnum.MultiSelect:
                                {
                                    foreach (var option in question.MultiSelectOptions)
                                    {
                                        var correspondingAnswer = multiAnswerList.FirstOrDefault(a => a.QuestionId == question.QuestionId && a.QuestionChoiceId == option.OptionId);

                                        option.IsSelected = correspondingAnswer != null;
                                    }

                                    break;
                                }
                            case QuestionTypeEnum.ShortAnswer:
                                {
                                    question.Answer = answer.Answer ?? null;
                                    break;
                                }
                            case QuestionTypeEnum.PointBased:
                                {
                                    question.PointScored = answer.PointScored ?? 0;
                                    break;
                                }
                        }
                    }
                }
            }

            var viewModel = new QuestionsViewModel(questionsModel, validFilter.PageNumber, validFilter.PageSize, totalQuestions)
            {
                CampaignId = campaignId,
                StoreId = storeId,
                IsCompleted = true,
                IsActive = isActive,
            };

            return PartialView("_ViewAllQuestions", viewModel);
        }
    }
}
