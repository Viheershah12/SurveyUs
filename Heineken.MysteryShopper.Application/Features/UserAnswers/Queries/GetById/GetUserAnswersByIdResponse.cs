using System;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Application.Features.UserAnswers.Queries.GetById
{
    public class GetUserAnswersByIdResponse
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int CampaignId { get; set; }

        public int StoreId { get; set; }

        public string UserId { get; set; }

        public QuestionTypeEnum ResponseType { get; set; }

        public string? Answer { get; set; }

        public int? QuestionChoiceId { get; set; }

        public int? PointScored { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
