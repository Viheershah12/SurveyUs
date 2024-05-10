namespace SurveyUs.Application.Features.StoreMappings.Queries.GetById
{
    public class GetStoreMappingsByIdResponse
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
