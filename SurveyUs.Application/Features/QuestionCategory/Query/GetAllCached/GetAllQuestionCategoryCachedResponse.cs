using System;

namespace SurveyUs.Application.Features.QuestionCategory.Query.GetAllCached
{
    public class GetAllQuestionCategoryCachedResponse
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
