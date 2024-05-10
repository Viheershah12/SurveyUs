using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyUs.Domain.Entities
{
    public class QuestionMappings
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public int QuestionId { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
