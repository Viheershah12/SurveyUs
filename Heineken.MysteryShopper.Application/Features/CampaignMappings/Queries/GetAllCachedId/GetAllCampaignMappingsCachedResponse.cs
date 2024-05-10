namespace SurveyUs.Application.Features.CampaignMappings.Queries.GetAllCachedId
{
    public class GetAllCampaignMappingsCachedResponse
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string CampaignId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
