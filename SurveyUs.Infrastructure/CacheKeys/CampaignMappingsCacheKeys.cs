namespace SurveyUs.Infrastructure.CacheKeys
{
    public static class CampaignMappingsCacheKeys
    {
        public static string ListKey => "CampaignMappingsList";

        public static string GetKey(int campaignMappingsId)
        {
            return $"CampaignMappings-{campaignMappingsId}";
        }
        public static string GetStoreKey(int storeId)
        {
            return $"CampaignMappingsStore-{storeId}";
        }
    }
}
