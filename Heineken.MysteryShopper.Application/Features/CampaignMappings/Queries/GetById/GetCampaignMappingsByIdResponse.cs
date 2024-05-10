namespace SurveyUs.Application.Features.CampaignMappings.Queries.GetById
{
    public class GetCampaignMappingsByIdResponse
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string CampaignId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
