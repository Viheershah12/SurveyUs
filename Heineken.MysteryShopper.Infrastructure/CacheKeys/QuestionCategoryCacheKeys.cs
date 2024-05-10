namespace SurveyUs.Infrastructure.CacheKeys
{
    public class QuestionCategoryCacheKeys
    {
        public static string ListKey => "QuestionCategoryList";

        public static string GetKey(int categoryId)
        {
            return $"QuestionCategory-{categoryId}";
        }

        public static string GetDetailsKey(int categoryId)
        {
            return $"QuestionCategoryDetails-{categoryId}";
        }
    }
}
