using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Web.Areas.Admin.Models
{
    public class StoreViewModel
    {
        public int Id { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public StateEnum State { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public List<SelectListItem> StateDropdown { get; set; } = new List<SelectListItem>();
        public bool IsAssigned { get; set; }
        public bool IsCompletedQuiz { get; set; } //for usage on scorecard
        public string DataTableId {  get; set; }
    }
}
