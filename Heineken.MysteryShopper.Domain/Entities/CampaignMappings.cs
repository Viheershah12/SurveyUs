namespace SurveyUs.Domain.Entities
{
    public class CampaignMappings
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int CampaignId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set;}
    }
}
