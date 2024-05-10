namespace SurveyUs.Application.Features.QuestionCategoryMapping.Queries.GetById
{
    public class GetQuestionCategoryMappingByIdResponse
    {
        public int Id { get; set; }

        public int QuestionMappingId { get; set; }

        public int QuestionCategoryId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
