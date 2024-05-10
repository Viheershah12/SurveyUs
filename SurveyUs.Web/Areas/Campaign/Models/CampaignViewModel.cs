using System.Collections.Generic;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Campaign.Models
{
    public class CampaignViewModel
    {
        public string UserId { get; set; }
        public List<CampaignSettingViewModel> Campaigns { get; set; }
    }


}
