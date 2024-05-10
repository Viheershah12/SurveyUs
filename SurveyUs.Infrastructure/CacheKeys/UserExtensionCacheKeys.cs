namespace SurveyUs.Infrastructure.CacheKeys
{
    public static class UserExtensionCacheKeys
    {
        public static string ListKey => "UserExtensionList";

        public static string SelectListKey => "UserExtensionSelectList";

        public static string GetKey(int userExtensionId)
        {
            return $"UserExtension-{userExtensionId}";
        }

        public static string GetDetailsKey(int userExtensionId)
        {
            return $"UserExtensionDetails-{userExtensionId}";
        }
    }
}