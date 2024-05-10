namespace SurveyUs.Application.Features.QuestionCategoryMapping.Queries.GetAllCached
{
    public class GetAllQuestionCategoryMappingCachedResponse
    {
        public int Id { get; set; }

        public int QuestionMappingId { get; set; }

        public int QuestionCategoryId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
