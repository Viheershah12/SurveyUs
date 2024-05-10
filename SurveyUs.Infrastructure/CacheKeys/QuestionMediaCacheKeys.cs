namespace SurveyUs.Infrastructure.CacheKeys
{
    public class QuestionMediaCacheKeys
    {
        public static string ListKey => "QuestionMedia";

        public static string GetKey(int questionMediaId)
        {
            return $"QuestionMedia-{questionMediaId}";
        }

        public static string GetDetailsKey(int questionMediaId)
        {
            return $"QuestionMediaDetails-{questionMediaId}";
        }
    }
}
