using System;

namespace SurveyUs.Application.Features.QuestionMedia.Queries.GetById
{
    public class GetQuestionMediaByIdResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
        public byte[] FileBinary { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; }
    }
}
