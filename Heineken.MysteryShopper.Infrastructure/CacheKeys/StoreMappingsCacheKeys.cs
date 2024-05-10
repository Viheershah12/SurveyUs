namespace SurveyUs.Infrastructure.CacheKeys
{
    public static class StoreMappingsCacheKeys
    {
        public static string ListKey => "StoreMappingList";

        public static string SelectListKey => "StoreMappingSelectList";

        public static string GetKey(int storeMappingId)
        {
            return $"StoreMapping-{storeMappingId}";
        }

        public static string GetDetailsKey(int storeMappingId)
        {
            return $"StoreMappingDetails-{storeMappingId}";
        }
    }
}
