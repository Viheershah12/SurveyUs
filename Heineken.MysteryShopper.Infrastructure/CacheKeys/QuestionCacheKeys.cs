namespace SurveyUs.Infrastructure.CacheKeys
{
    public class QuestionCacheKeys
    {
        public static string ListKey => "QuestionList";

        public static string GetKey(int questionId)
        {
            return $"Question-{questionId}";
        }

        public static string GetDetailsKey(int questionId)
        {
            return $"QuestionDetails-{questionId}";
        }
    }
}
