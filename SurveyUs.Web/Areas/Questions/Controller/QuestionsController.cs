using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Application.Features.QuestionAnswers.Queries.GetAll;
using SurveyUs.Application.Features.QuestionMedia.Commands.Create;
using SurveyUs.Application.Features.QuestionMedia.Queries.GetById;
using SurveyUs.Application.Features.Store.Queries.GetById;
using SurveyUs.Application.Features.UserAnswers.Commands.Create;
using SurveyUs.Application.Features.UserAnswers.Queries.GetById;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Areas.Questions.Model;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Questions.Controller
{
    [Area("Questions")]

    public class QuestionsController : BaseController<QuestionsController>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionMappingsRepository _questionMappingsRepository;
        private readonly IQuestionChoicesRepository _questionChoicesRepository;
        private readonly IUserAnswersRepository _userAnswersRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionsController(
            IQuestionRepository questionRepository,
            IQuestionMappingsRepository questionMappingsRepository,
            IQuestionChoicesRepository questionChoicesRepository,
            IUserAnswersRepository userAnswersRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            _questionRepository = questionRepository;
            _questionMappingsRepository = questionMappingsRepository;
            _questionChoicesRepository = questionChoicesRepository;
            _userAnswersRepository = userAnswersRepository;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(int campaignId, int storeId)
        {
            var campaignDetail = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });
            var storeDetail = await _mediator.Send(new GetStoreByIdQuery() { Id = storeId });

            var viewModel = new QuestionsViewModel(null, 1, 5, 0)
            {
                CampaignId = campaignId,
                CampaignName = campaignDetail.Data.Name,
                StoreId = storeId,
                StoreName = storeDetail.Data.Name
            };

            return View(viewModel);
        }

        public async Task<IActionResult> LoadAll(int campaignId, int storeId, int pageNumber = 1, int pageSize = 5)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

            var completedQuizAnswers = _userAnswersRepository.UserAnswers.Where(a => a.CampaignId == campaignId && a.StoreId ==  storeId && a.UserId == userId).ToList();
            bool quizCompleted = false;
            if (completedQuizAnswers.Count != 0)
            {
                quizCompleted = true;
                foreach (var question in questionsModel)
                {
                    var answer = new UserAnswers();
                    var multiAnswerList = new List<UserAnswers>();
                        
                    if(question.ResponseType != QuestionTypeEnum.MultiSelect)
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
            else
            {
                try
                {
                    var savedAnswers = await _mediator.Send(new GetSavedAnswersByPageNumberQuery { StoreId = storeId, CampaignId = campaignId, UserId = userId, PageNumber = pageNumber });
                    var getCachedFiles = await _mediator.Send(new GetSavedFileMediaByPageNumberQuery { StoreId = storeId, CampaignId = campaignId, UserId = userId, PageNumber = pageNumber });

                    if (savedAnswers.Succeeded && savedAnswers.Data != null)
                    {
                        var savedAnswersDictionary = savedAnswers.Data.GroupBy(x => x.QuestionId).ToDictionary(x => x.Key, y => y.ToList());

                        for (int i = 0; i < savedAnswersDictionary.Count; i++)
                        {
                            var answers = savedAnswersDictionary.Where(x => x.Key == questionsModel[i].QuestionId).Select(x => x.Value).FirstOrDefault();

                            var savedFiles = new List<GetQuestionMediaByIdResponse>();

                            if (getCachedFiles.Succeeded)
                            {
                                savedFiles = getCachedFiles.Data;
                            }

                            var questionMediaSavedFiles = savedFiles.Where(x => x.QuestionId == answers?[i].QuestionId).Select(x => new QuestionMediaFile
                            {
                                Id = x.Id,
                                QuestionId = x.QuestionId,
                                ContentType = x.ContentType,
                                Extension = x.Extension,
                                Name = x.Name,
                                FileBinary = x.FileBinary
                            }).ToList();

                            questionsModel[i].QuestionMediaList = questionMediaSavedFiles;

                            switch (questionsModel[i].ResponseType)
                            {
                                case QuestionTypeEnum.TrueFalse:
                                case QuestionTypeEnum.MultipleChoice:
                                    {
                                        questionsModel[i].QuestionChoiceId = answers[0].QuestionChoiceId ?? 0;
                                        break;
                                    }
                                case QuestionTypeEnum.MultiSelect:
                                    {
                                        foreach (var option in questionsModel[i].MultiSelectOptions)
                                        {
                                            var correspondingAnswer = answers.FirstOrDefault(a => a.QuestionId == questionsModel[i].QuestionId && a.QuestionChoiceId == option.OptionId);

                                            option.IsSelected = correspondingAnswer != null;
                                        }

                                        break;
                                    }
                                case QuestionTypeEnum.ShortAnswer:
                                    {
                                        questionsModel[i].Answer = answers[i].Answer ?? null;
                                        break;
                                    }
                                case QuestionTypeEnum.PointBased:
                                    {
                                        questionsModel[i].PointScored = answers[i].PointScored ?? 0;
                                        break;
                                    }
                            }
                        }
                    }


                }
                catch (Exception ex)
                {

                }
            }

            var viewModel = new QuestionsViewModel(questionsModel, validFilter.PageNumber, validFilter.PageSize, totalQuestions)
            {
                CampaignId = campaignId,
                StoreId = storeId,
                IsCompleted = quizCompleted,
                IsActive = isActive
            };

            return PartialView("_ViewAll", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuestionnaire(QuestionsViewModel form)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var answers = _mapper.Map<List<GetUserAnswersByIdResponse>>(form.Data.Where(x => x.MultiSelectOptions.Count == 0).ToList());

            var multiSelectOptions = form.Data.Where(x => x.MultiSelectOptions.Count > 0).SelectMany(x => x.MultiSelectOptions.Where(opt => opt.IsSelected), (x, opt) => new GetUserAnswersByIdResponse
            {
                QuestionId = x.QuestionId,
                QuestionChoiceId = opt.OptionId,
                ResponseType = QuestionTypeEnum.MultiSelect
            }).ToList();

            answers.AddRange(multiSelectOptions);

            var createModel = new CreateSavedAnswersCommand
            {
                StoreId = form.StoreId,
                CampaignId = form.CampaignId,
                PageNumber = form.PageNumber,
                UserId = userId,
                Answers = answers,
            };
            var result = await _mediator.Send(createModel);

            var getPrevCachedFiles = await _mediator.Send(new GetSavedFileMediaByPageNumberQuery { StoreId = form.StoreId, CampaignId = form.CampaignId, UserId = userId, PageNumber = form.PageNumber });

            foreach (var qMedia in form.Data)
            {
                var cachedMediaList = new List<CreateQuestionMediaCommand>();

                if (getPrevCachedFiles.Succeeded)
                {
                    var prevQuestionMedia = getPrevCachedFiles.Data.Where(x => x.QuestionId == qMedia.QuestionId)
                        .Select(x => new CreateQuestionMediaCommand
                        {
                            QuestionId = x.QuestionId,
                            Name = x.Name,
                            Extension = x.Extension,
                            ContentType = x.ContentType,
                            FileBinary = x.FileBinary,
                            CreatedBy = userId,
                            CreatedOn = DateTime.Now,
                            IsDeleted = false,
                            CampaignId = form.CampaignId,
                            StoreId = form.StoreId,
                            UserId = userId
                        }).ToList();

                    cachedMediaList.AddRange(prevQuestionMedia);
                }

                var questionMedia = form.Data.Where(x => x.QuestionId == qMedia.QuestionId)
                    .Select(x => (x.QuestionId, x.QuestionMedia)).ToList();

                foreach (var file in questionMedia)
                {
                    if (file.QuestionMedia != null)
                    {
                        foreach (var media in file.QuestionMedia)
                        {
                            var mediaFile = new CreateQuestionMediaCommand
                            {
                                QuestionId = file.QuestionId,
                                Name = media.FileName,
                                Extension = media.ContentType.Split("/")[0],
                                ContentType = media.ContentType,
                                FileBinary = (await GetBytes(media))[0],
                                CreatedBy = userId,
                                CreatedOn = DateTime.Now,
                                IsDeleted = false,
                                CampaignId = form.CampaignId,
                                StoreId = form.StoreId,
                                UserId = userId
                            };
                            cachedMediaList.Add(mediaFile);
                        }
                    }
                }

                if (cachedMediaList.Count > 0)
                {
                    var createQuestionMedia = new CreateSavedMediaCommand
                    {
                        StoreId = form.StoreId,
                        CampaignId = form.CampaignId,
                        PageNumber = form.PageNumber,
                        UserId = userId,
                        QuestionMedia = cachedMediaList
                    };

                    await _mediator.Send(createQuestionMedia);
                }
            }

            if (form.IsSave)
            {
                return RedirectToAction("LoadAll", new { campaignId = form.CampaignId, storeId = form.StoreId, pageNumber = form.NewPageNumber });
            }

            var allAnswers = new List<GetUserAnswersByIdResponse>();
            var allFiles = new List<GetQuestionMediaByIdResponse>();

            for (var i = 1; i <= form.TotalPages; i++)
            {
                var getCachedAnswers = await _mediator.Send(new GetSavedAnswersByPageNumberQuery { StoreId = form.StoreId, CampaignId = form.CampaignId, UserId = userId, PageNumber = i });
                var answerByPage = getCachedAnswers.Data;

                var getCachedFiles = await _mediator.Send(new GetSavedFileMediaByPageNumberQuery { StoreId = form.StoreId, CampaignId = form.CampaignId, UserId = userId, PageNumber = i } );
                var filesPerPage = getCachedFiles.Data;

                if (answerByPage != null)
                {
                    allAnswers.AddRange(answerByPage);
                    allFiles.AddRange(filesPerPage);
                }
                else
                {
                    return RedirectToAction("LoadAll", new { campaignId = form.CampaignId, storeId = form.StoreId, pageNumber = i });
                }
            }

            var userAnswers = _mapper.Map<List<UserAnswers>>(allAnswers);
            var userFiles = _mapper.Map<List<QuestionMedia>>(allFiles);

            try
            {
                var createUserAnswerCommand = new CreateUserAnswersCommand();
                var rewardQuestions = new List<UserAnswers>();
                var questionAnswers = new List<GetQuestionAnswersByQuestionIdResponse>();

                foreach (var userAnswer in userAnswers)
                {
                    userAnswer.StoreId = form.StoreId;
                    userAnswer.CampaignId = form.CampaignId;
                    userAnswer.UserId = userId;

                    var questionAnswersCommand = await _mediator.Send(new GetQuestionAnswersByQuestionId { QuestionId = userAnswer.QuestionId });

                    if (questionAnswersCommand.Succeeded)
                        questionAnswers.AddRange(questionAnswersCommand.Data);
                }

                createUserAnswerCommand.Answers = userAnswers;
                var response = await _mediator.Send(createUserAnswerCommand);

                if (response.Succeeded)
                {
                    rewardQuestions.AddRange(_userAnswersRepository.UserAnswers.Include(x => x.Question)
                        .Where(x =>
                            x.CampaignId == form.CampaignId &&
                            x.StoreId == form.StoreId &&
                            x.UserId == userId &&
                            x.Question.HasReward)
                        .ToList());

                    var groupedUserAnswers = rewardQuestions.GroupBy(x => x.QuestionId)
                        .ToDictionary(x => x.Key, y => y.ToList());

                    var groupedQuestionAnswers = questionAnswers.GroupBy(x => x.QuestionId)
                        .ToDictionary(x => x.Key, y => y.ToList());

                    var isCorrect = true;

                    foreach(var answer in groupedUserAnswers.Where(x => x.Value.Count == 1))
                    {
                        var filteredQuestionAnswers = groupedQuestionAnswers.Where(x => x.Key == answer.Key && x.Value.Count == 1)
                            .Select(x => x.Value.FirstOrDefault())
                            .FirstOrDefault();

                        if(filteredQuestionAnswers == null)
                            continue;

                        var answerValue = answer.Value.FirstOrDefault();

                        if (answerValue == null)
                            continue;

                        if (filteredQuestionAnswers.QuestionChoiceId != answerValue.QuestionChoiceId)
                            isCorrect = false;
                    }

                    foreach (var multiSelectAnswer in groupedQuestionAnswers.Where(x => x.Value.Count > 1))
                    {
                        // Get single-select answers for the same question
                        var singleSelectAnswers = groupedQuestionAnswers.Where(x => x.Key == multiSelectAnswer.Key && x.Value.Count == 1)
                            .Select(x => x.Value.FirstOrDefault())
                            .ToList();

                        // Now, iterate through each single-select answer and compare with the multi-select answers
                        foreach (var singleAnswer in singleSelectAnswers)
                        {
                            foreach (var multiAnswer in multiSelectAnswer.Value)
                            {
                                if (singleAnswer.QuestionChoiceId != multiAnswer.QuestionChoiceId)
                                    isCorrect = false;
                            }
                        }
                    }

                    if (isCorrect)
                        _notify.Information("Congratulations you are eligible for a RM50 reward via touch n go / grab / QR Pay ", 60);

                    _notify.Success("Successfully Submitted");

                    foreach (var file in userFiles)
                    {
                        var mappedQuestionMedia = _mapper.Map<CreateQuestionMediaCommand>(file);
                        await _mediator.Send(mappedQuestionMedia);
                    }
                }
            }
            catch (Exception ex)
            {
                _notify.Error("Failed To Submit");
            }

            return await LoadAll(form.CampaignId, form.StoreId, 1);

            #region oldCode
            //var campaignId = 0;
            //var storeId = 0;

            //try
            //{
            //    foreach (var key in form.Keys)
            //    {
            //        if (key.StartsWith("Questions") && key.Contains(".SelectedOption"))
            //        {
            //            try
            //            {
            //                var index = GetIndexFromKey(key);
            //                var selectedOptionValue = form[key].ToString();

            //                // Split the combined value into QuestionId and OptionId
            //                var parts = selectedOptionValue.Split('-');
            //                var questionId = int.Parse(parts[0]);
            //                var selectedOption = int.Parse(parts[1]);
            //                campaignId = int.Parse(parts[2]);
            //                storeId = int.Parse(parts[3]);

            //                var question = await _questionRepository.GetByIdAsync(questionId);
            //                var i = 0;

            //                while (i <= index)
            //                {
            //                    var userAnswer = new UserAnswers()
            //                    {
            //                        CampaignId = campaignId,
            //                        QuestionId = questionId,
            //                        QuestionChoiceId = selectedOption,
            //                        UserId = (await _userManager.GetUserAsync(HttpContext.User)).Id,
            //                        StoreId = storeId,
            //                        ResponseType = ResponseTypeEnum.TrueFalse,
            //                        PointScored = question.Points,
            //                        CreatedOn = DateTime.Now,
            //                        UpdatedOn = DateTime.Now,
            //                        IsDeleted = false
            //                    };

            //                    var userAnswerCommand = _mapper.Map<CreateUserAnswersCommand>(userAnswer);
            //                    await _mediator.Send(userAnswerCommand);

            //                    i++;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine($"Error processing key: {key}, Error: {ex.Message}");
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return View("Error");
            //}

            //return RedirectToAction("Index"); //, new { campaignId = campaignId, storeId = storeId }
            #endregion
        }

        public static async Task<List<byte[]>> GetBytes(IFormFile formFile)
        {

            var newList = new List<byte[]>();

            if (formFile.ContentType != "application/pdf")
            {
                newList.Add(formFile.OptimizeImageSize(700, 700));
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    newList.Add(memoryStream.ToArray());
                    memoryStream.Position = 0;
                }
            }
            return newList;

        }

        //private int GetIndexFromKey(string key)
        //{
        //    var keyWithoutPrefix = key.Split(".")[0].Substring("Questions".Length);
        //    var indexStart = keyWithoutPrefix.IndexOf('[');
        //    var indexEnd = keyWithoutPrefix.IndexOf(']');

        //    if (indexStart != -1 && indexEnd != -1)
        //    {
        //        var indexString = keyWithoutPrefix.Substring(indexStart + 1, indexEnd - indexStart - 1);
        //        if (int.TryParse(indexString, out var index))
        //        {
        //            return index;
        //        }
        //    }

        //    throw new FormatException($"Invalid index format in key: {key}");
        //}
    }
}
