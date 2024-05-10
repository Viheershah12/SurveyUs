using System;

namespace SurveyUs.Application.Features.QuestionChoices.Queries.GetAllCached
{
    public class GetAllQuestionChoicesCachedResponse
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string ChoiceText { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
