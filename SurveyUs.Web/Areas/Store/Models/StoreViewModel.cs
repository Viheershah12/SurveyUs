using System.Collections.Generic;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Store.Models
{
    public class ScoreboardStoreViewModel
    {
        public int CampaignId {  get; set; }
        public string CampaignName { get; set; }
        public List<StoreViewModel> Stores { get; set; }
    }
}
