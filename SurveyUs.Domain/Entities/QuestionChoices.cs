using System;

namespace SurveyUs.Domain.Entities
{
    public class QuestionChoices
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string ChoiceText { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
