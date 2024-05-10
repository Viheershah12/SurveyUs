using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SurveyUs.Domain.Enums;
using SurveyUs.Web.Models;

namespace SurveyUs.Web.Areas.Questions.Model
{
    public class QuestionsViewModel : PagedResponse<List<QuestionsModel>>
    {
        public QuestionsViewModel(List<QuestionsModel>? data, int pageNumber, int pageSize, int totalRecords) : base(data, pageNumber, pageSize, totalRecords)
        {
        }

        public QuestionsViewModel() { }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public int NewPageNumber { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public bool IsSave { get; set; } = false;

        //variables below are used in UserSubmissionController
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public string CampaignName { get; set; }
        public DateTime SubmissionTime { get; set; }
    }

    public class QuestionsModel
    {
        public int QuestionId { get; set; }

        public QuestionTypeEnum ResponseType { get; set; }

        public string QuestionText { get; set; }

        public int Points { get; set; }

        public int? QuestionChoiceId { get; set; } = null;

        public double PointScored { get; set; }

        public string Answer { get; set; }

        public List<AnswerOption> AnswerOptions { get; set; }

        public bool HasMedia { get; set; } = false;

        [DataType(DataType.Upload)]
        public List<IFormFile> QuestionMedia { get; set; }

        public List<QuestionMediaFile> QuestionMediaList { get; set; }

        public List<MultiSelect> MultiSelectOptions { get; set; } = new();
    }

    public class QuestionMediaFile
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public byte[] FileBinary { get; set; }
    }


    public class AnswerOption
    {
        public int OptionId { get; set; }

        public string OptionText { get; set; }
    }

    public class MultiSelect
    {
        public int? OptionId { get; set; }

        public string OptionText { get; set; }

        public bool IsSelected { get; set; }
    }

}
