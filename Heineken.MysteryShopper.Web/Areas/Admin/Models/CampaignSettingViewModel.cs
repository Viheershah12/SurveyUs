using System;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Web.Areas.Admin.Models
{

    //public class CampaignViewModel
    //{
    //    public int StoreId { get; set; }

    //    public List<CampaignSettingViewModel> CampaignList { get; set; }
    //}

    public class CampaignSettingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusString { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime StartDate { get; set; }
        public string StartDateString { get; set; }
        public DateTime EndDate { get; set; }
        public string EndDateString { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string DataTableId {  get; set; }
    }
}
