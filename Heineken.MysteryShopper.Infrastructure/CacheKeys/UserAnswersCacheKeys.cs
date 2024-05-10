namespace SurveyUs.Infrastructure.CacheKeys
{
    public class UserAnswersCacheKeys
    {
        public static string ListKey => "UserAnswersList";

        public static string GetKey(int userAnswerId)
        {
            return $"UserAnswers-{userAnswerId}";
        }

        public static string GetDetailsKey(int userAnswerId)
        {
            return $"UserAnswersDetails-{userAnswerId}";
        }

        public static string GetSaveKey(int storeId, int campaignId, string userId, int pageNumber)
        {
            return $"UserAnswersByPage-{storeId}-{campaignId}-{userId}-{pageNumber}";
        }

        public static string GetFileSaveKey(int storeId, int campaignId, string userId, int pageNumber)
        {
            return $"UserQuestionMediaByPage-{storeId}-{campaignId}-{userId}-{pageNumber}";
        }
    }
}
