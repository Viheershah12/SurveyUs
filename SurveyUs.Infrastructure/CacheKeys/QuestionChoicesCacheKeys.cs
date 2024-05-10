namespace SurveyUs.Infrastructure.CacheKeys
{
    public class QuestionChoicesCacheKeys
    {
        public static string ListKey => "QuestionChoicesList";

        public static string GetKey(int questionChoiceId)
        {
            return $"QuestionChoices-{questionChoiceId}";
        }

        public static string GetDetailsKey(int questionChoiceId)
        {
            return $"QuestionChoicesDetails-{questionChoiceId}";
        }
    }
}
