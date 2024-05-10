namespace SurveyUs.Application.Features.QuestionMappings.Queries.Queries
{
    public class GetQuestionMappingsByIdResponse
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public int QuestionId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
