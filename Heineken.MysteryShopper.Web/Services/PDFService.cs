using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.PDF.Models;
using Wkhtmltopdf.NetCore;

namespace SurveyUs.Web.Services
{
    [Area("PDFService")]
    public class PDFService : IPDFService
    {
        private const string _globalScorePDFTemplate = "~/Areas/PDF/Views/PDF/GlobalScorePDFTemplate.cshtml";
        private const string _averageScoreByStorePDFTemplate = "~/Areas/PDF/Views/PDF/AverageScoreByStorePDFTemplate.cshtml";
        private const string _scoreByAssessmentPDFTemplate = "~/Areas/PDF/Views/PDF/ScoreByAssessmentPDFTemplate.cshtml";
        private const string _scoreByStoreAssessmentPDFTemplate = "~/Areas/PDF/Views/PDF/ScoreByStoreAssessmentPDFTemplate.cshtml";
        private const string _AllUsersPDFTemplate = "~/Areas/PDF/Views/PDF/AllUsersPDFTemplate.cshtml";
        private const string _allBartendersPDFTemplate = "~/Areas/PDF/Views/PDF/AllBartendersPDFTemplate.cshtml";
        private const string _allJudgesPDFTemplate = "~/Areas/PDF/Views/PDF/AllJudgesPDFTemplate.cshtml";
        private readonly IGeneratePdf _generatePDF;
        private readonly IViewRenderService _viewRenderService;

        public PDFService(IGeneratePdf generatePDF, IViewRenderService viewRenderService)
        {
            _generatePDF = generatePDF;
            _viewRenderService = viewRenderService;
        }

        public async Task PrintGlobalScorePDF(Stream stream, List<PDFModel> report)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _generatePDF.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 10 }
            });

            var html = await _viewRenderService.RenderToStringAsync(_globalScorePDFTemplate, report);
            var pdfBytes = _generatePDF.GetPDF(html);
            stream.Write(pdfBytes);
        }

        public async Task PrintAverageScoreByStorePDF(Stream stream, List<PDFModel> report)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _generatePDF.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 10 }
            });

            var html = await _viewRenderService.RenderToStringAsync(_averageScoreByStorePDFTemplate, report);
            var pdfBytes = _generatePDF.GetPDF(html);
            stream.Write(pdfBytes);
        }

        public async Task PrintScoreByAssessmentPDF(Stream stream, List<PDFModel> report)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _generatePDF.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 10 }
            });

            var html = await _viewRenderService.RenderToStringAsync(_scoreByAssessmentPDFTemplate, report);
            var pdfBytes = _generatePDF.GetPDF(html);
            stream.Write(pdfBytes);
        }
        public async Task PrintScoreByStoreAssessmentPDF(Stream stream, List<PDFModel> report)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _generatePDF.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 10 }
            });

            var html = await _viewRenderService.RenderToStringAsync(_scoreByStoreAssessmentPDFTemplate, report);
            var pdfBytes = _generatePDF.GetPDF(html);
            stream.Write(pdfBytes);
        }

        public async Task PrintAllUsers(Stream stream, List<PDFModel> report)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _generatePDF.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 10 }
            });

            var html = await _viewRenderService.RenderToStringAsync(_AllUsersPDFTemplate, report);
            var pdfBytes = _generatePDF.GetPDF(html);
            stream.Write(pdfBytes);
        }

        public async Task PrintAllBartenders(Stream stream, List<BartenderPDFModel> report)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _generatePDF.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Landscape,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 10 }
            });

            var html = await _viewRenderService.RenderToStringAsync(_allBartendersPDFTemplate, report);
            var pdfBytes = _generatePDF.GetPDF(html);
            stream.Write(pdfBytes);
        }

        public async Task PrintAllJudges(Stream stream, List<JudgePDFModel> report)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _generatePDF.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 10 }
            });

            var html = await _viewRenderService.RenderToStringAsync(_allJudgesPDFTemplate, report);
            var pdfBytes = _generatePDF.GetPDF(html);
            stream.Write(pdfBytes);
        }
    }
}