namespace SurveyUs.Infrastructure.CacheKeys
{
    public class QuestionAnswerCacheKeys
    {
        public static string ListKey => "QuestionAnswerList";

        public static string GetKey(int questionAnswerId)
        {
            return $"QuestionAnswer-{questionAnswerId}";
        }

        public static string GetDetailsKey(int questionAnswerId)
        {
            return $"QuestionAnswerDetails-{questionAnswerId}";
        }
    }
}
