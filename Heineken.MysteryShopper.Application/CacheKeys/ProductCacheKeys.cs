namespace SurveyUs.Application.CacheKeys
{
    public static class ProductCacheKeys
    {
        public static string ListKey => "ProductList";

        public static string SelectListKey => "ProductSelectList";

        public static string GetKey(int productId)
        {
            return $"Product-{productId}";
        }

        public static string GetDetailsKey(int productId)
        {
            return $"ProductDetails-{productId}";
        }
    }
}