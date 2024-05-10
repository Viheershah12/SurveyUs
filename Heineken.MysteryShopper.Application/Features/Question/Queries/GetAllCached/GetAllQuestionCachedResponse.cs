using System;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Application.Features.Question.Queries.GetAllCached
{
    public class GetAllQuestionCachedResponse
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public QuestionTypeEnum ResponseType { get; set; }

        public string QuestionText { get; set; }

        public int Points { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public bool HasMedia { get; set; }

        public bool HasReward { get; set; }

        public int DisplayOrder { get; set; }
    }
}
