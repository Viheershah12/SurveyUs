using SurveyUs.Domain.Enums;

namespace SurveyUs.Web.Areas.PDF.Models
{
    public class PDFModel
    {
        public string Name { get; set; }
        public string Outlet { get; set; }
        public StateEnum State { get; set; }
        public int Score { get; set; }
        public int BartenderCount { get; set; }
        public string testType { get; set; }
        public string RepresentAs { get; set; }
        public string OutletName { get; set; }
    }

    public class BartenderPDFModel
    {
        public string Name { get; set; }
        public string IC { get; set; }
        public string PhoneNo { get; set; }
        public string Gender { get; set; }
        public string OutletName { get; set; }
        public string OutletAddress { get; set; }
        public string OutletLocation { get; set; }
        public string JoiningAs { get; set; }
        public string Designation { get; set; }
        public string ShirtSize { get; set; }
    }

    public class JudgePDFModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}