using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Web.Areas.Admin.Models
{
    public class QuestionViewModel
    {
        public QuestionViewModel()
        {
            QuestionTypeDropdown = new List<SelectListItem>();
            QuestionCategoryDropdown = new List<SelectListItem>();
            Options = new List<string>();
        }

        public int Id { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public QuestionTypeEnum ResponseType { get; set; }
        public string ResponseTypeString {  get; set; }

        public string QuestionText { get; set; }

        public int Points { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public List<SelectListItem> QuestionTypeDropdown { get; set; }

        public List<SelectListItem> QuestionCategoryDropdown { get; set; }

        public List<string> Options { get; set; }

        public bool IsAssigned {  get; set; } = false;

        public bool HasMedia { get; set; }

        public bool HasReward { get; set; }

        public int DisplayOrder { get; set; }

        public List<QuestionAnswers> QuestionOptions { get; set; } = new();
    }

    public class QuestionAnswers
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int QuestionChoiceId { get; set; }

        public string ChoiceText { get; set; }

        public bool IsCorrect { get; set; }
    }

    public class QuestionCategoryViewModel
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedOnString { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string DataTableId {  get; set; }
    }
}
