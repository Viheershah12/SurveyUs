using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Models
{
    public class QuestionMappingViewModel
    {
        public QuestionMappingViewModel()
        {
            QuestionCategoryList = new List<SelectListItem>();
            QuestionList = new List<QuestionViewModel>();
        }

        public int CampaignId { get; set; }

        public int CategoryId { get; set; }

        public string CampaignName { get; set; }

        public List<int> QuestionIds { get; set; }

        public List<SelectListItem> QuestionCategoryList { get; set; }

        public List<QuestionViewModel> QuestionList { get; set; }
    }

    public class QuestionSelectViewModel
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; }
    }

    public class QuestionMappingDataTable : DataTableExtensions
    {
        public int CampaignId { get; set; }
        public int CategoryId { get; set; }
    }
}
