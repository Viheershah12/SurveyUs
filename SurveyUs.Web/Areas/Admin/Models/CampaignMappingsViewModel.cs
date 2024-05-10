using System.Collections.Generic;
using SurveyUs.Web.Extensions;
using SurveyUs.Web.Models;

namespace SurveyUs.Web.Areas.Admin.Models
{
    public class CampaignMappingsCreateModel
    {
        public int StoreId { get; set; }
        public List<CampaignSettingViewModel> Campaigns { get; set; } = new List<CampaignSettingViewModel>();
    }
    public class CampaignMappingsViewModel
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set;}
    }

    public class PagedCampaignMappingsViewModel : PagedResponse<List<StoreViewModel>>
    {
        public PagedCampaignMappingsViewModel(int campaignId, int pageNumber, int pageSize) 
        {
            CampaignId = campaignId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = new List<StoreViewModel>();
        }

        public PagedCampaignMappingsViewModel(List<StoreViewModel> data, int pageNumber, int pageSize, int totalRecords) : base(data, pageNumber, pageSize, totalRecords) { }
        public int CampaignId { get; set; }
    }

    public class CampaignMappingsDataTable : DataTableExtensions
    {
        public int CampaignId { get; set; }
    }
}
