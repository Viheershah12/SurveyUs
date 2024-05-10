namespace SurveyUs.Domain.Entities
{
    public class StoreMappings
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set;}
    }
}
