namespace SurveyUs.Infrastructure.CacheKeys
{
    public class QuestionCategoryMappingCacheKeys
    {
        public static string ListKey => "QuestionCategoryMappingList";

        public static string GetKey(int categoryMappingId)
        {
            return $"QuestionCategoryMapping-{categoryMappingId}";
        }

        public static string GetDetailsKey(int categoryMappingId)
        {
            return $"QuestionCategoryMappingDetails-{categoryMappingId}";
        }
    }
}
