using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SurveyUs.Web.Areas.PDF.Models;

namespace SurveyUs.Web.Abstractions
{
    public interface IPDFService
    {
        Task PrintGlobalScorePDF(Stream stream, List<PDFModel> report);

        Task PrintAverageScoreByStorePDF(Stream stream, List<PDFModel> report);

        Task PrintScoreByAssessmentPDF(Stream stream, List<PDFModel> report);

        Task PrintScoreByStoreAssessmentPDF(Stream stream, List<PDFModel> report);

        Task PrintAllUsers(Stream stream, List<PDFModel> report);

        Task PrintAllBartenders(Stream stream, List<BartenderPDFModel> report);

        Task PrintAllJudges(Stream stream, List<JudgePDFModel> report);
    }
}