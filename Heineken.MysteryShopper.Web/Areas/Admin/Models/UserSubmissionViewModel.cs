using System.Collections.Generic;

namespace SurveyUs.Web.Areas.Admin.Models
{
    public class UserSubmissionViewModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public List<UserSubmission> UserSubmissions { get; set;}
    }

    public class UserSubmission
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public int StoreId { get; set; }
    }
}
