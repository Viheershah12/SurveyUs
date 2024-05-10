using System;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Domain.Entities
{
    public class Store
    {
        public int Id { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public StateEnum State { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set;}
    }
}