namespace SurveyUs.Infrastructure.CacheKeys
{
    public static class StoreCacheKeys
    {
        public static string ListKey => "StoreList";

        public static string SelectListKey => "StoreSelectList";

        public static string GetKey(int storeId)
        {
            return $"Store-{storeId}";
        }

        public static string GetDetailsKey(int storeId)
        {
            return $"StoreDetails-{storeId}";
        }
    }
}