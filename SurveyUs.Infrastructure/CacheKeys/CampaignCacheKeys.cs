namespace SurveyUs.Infrastructure.CacheKeys
{
    public static class CampaignCacheKeys
    {
        public static string ListKey => "CampaignList";

        public static string GetKey(int campaignId)
        {
            return $"Campaign-{campaignId}";
        }

        public static string GetDetailsKey(int campaignId)
        {
            return $"CampaignDetails-{campaignId}";
        }
    }
}
