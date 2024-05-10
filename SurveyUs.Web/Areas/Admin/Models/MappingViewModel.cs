using System.Collections.Generic;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Models
{
    public class StoresMappingViewModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public List<StoreViewModel> Stores { get; set; } = new List<StoreViewModel>();
    }

    public class ShopperMappingViewModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public List<UserViewModel> MShopperMapping { get; set; } = new List<UserViewModel>();
    }

    public class MysteryShopperDataTable : DataTableExtensions
    {
        public int StoreId { get; set; }
    }
}
