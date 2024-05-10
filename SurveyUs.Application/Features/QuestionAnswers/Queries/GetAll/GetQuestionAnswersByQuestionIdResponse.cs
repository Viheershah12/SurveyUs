using System;

namespace SurveyUs.Application.Features.QuestionAnswers.Queries.GetAll
{
    public class GetQuestionAnswersByQuestionIdResponse
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int QuestionChoiceId { get; set; }

        public string Answer { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
