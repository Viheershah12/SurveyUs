using System.Collections.Generic;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Web.Areas.Dashboard.Models
{
    public class ScoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StateEnum State { get; set; }
        public int Score { get; set; }
        public int BartenderCount { get; set; }
        public string RepresentAs { get; set; }
        public string OutletName { get; set; }
        public List<StoreOption> StoreOptions { get; set; }
    }

    public class StoreOption
    {
        public StateEnum State { get; set; }
    }
}