namespace SurveyUs.Infrastructure.CacheKeys
{
    public class QuestionMappingsCacheKeys
    {
        public static string ListKey => "QuestionMappingsList";

        public static string GetKey(int questionMappingId)
        {
            return $"QuestionMappings-{questionMappingId}";
        }

        public static string GetDetailsKey(int questionMappingId)
        {
            return $"QuestionMappingsDetails-{questionMappingId}";
        }
    }
}
