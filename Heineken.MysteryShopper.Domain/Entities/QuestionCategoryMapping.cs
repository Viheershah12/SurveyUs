namespace SurveyUs.Domain.Entities
{
    public class QuestionCategoryMapping
    {
        public int Id { get; set; }

        public int QuestionMappingId { get; set; }

        public int QuestionCategoryId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
