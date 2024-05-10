using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.PDF.Models;

namespace SurveyUs.Web.Areas.PDF.Controllers
{
    [Area("PDF")]
    public class PDFController : BaseController<PDFController>
    {
        private readonly IUserExtensionRepository _userExtensionRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPDFService _pdfService;

        public PDFController(IUserExtensionRepository userExtensionRepository,
            IStoreRepository storeRepository,
            UserManager<ApplicationUser> userManager,
            IPDFService pdfService
            )
        {
            _userExtensionRepository = userExtensionRepository;
            _storeRepository = storeRepository;
            _userManager = userManager;
            _pdfService = pdfService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GlobalScorePDF()
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            //var globalScores = await _globalScoreRepository.GetListAsync();

            //if (globalScores != null)
            //{
            //    foreach (var score in globalScores)
            //    {
            //        var user = await _userExtensionRepository.GetByUserIdAsync(score.UserId);
            //        var store = await _storeRepository.GetByIdAsync(user.Store);

            //        var model = new PDFModel()
            //        {
            //            Name = user.Name,
            //            State = store.State,
            //            Score = score.TotalScore,
            //            RepresentAs = user.JoiningAs,
            //            OutletName = user.Outlet
            //        };
            //        scoreModel.Add(model);
            //    }
            //}
            //scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            var report = scoreModel;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintGlobalScorePDF(stream, report);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "Global Score.pdf");
        }

        public async Task<IActionResult> AverageScoreByStorePDF()
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            var stores = await _storeRepository.GetListAsync();

            foreach (var store in stores)
            {
                int totalScore = 0;
                var bartenders = await _userExtensionRepository.GetByStoreAsync(store.Id);

                foreach (var bartender in bartenders)
                {
                    //var score = await _globalScoreRepository.GetByUserIdAsync(bartender.UserId);

                    //if (score != null)
                    //    totalScore += score.TotalScore;
                }

                var model = new PDFModel()
                {
                    State = store.State,
                    Score = totalScore / (bartenders.Count == 0 ? 1 : bartenders.Count),
                    BartenderCount = bartenders.Count
                };
                scoreModel.Add(model);
            }
            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            var report = scoreModel;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintAverageScoreByStorePDF(stream, report);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "Average Score by Store.pdf");
        }

        public async Task<IActionResult> ScoreByHeinekenAssessmentPDF()
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            //var scores = await _globalScoreRepository.GetListAsync();

            //foreach (var score in scores)
            //{
            //    var user = await _userManager.FindByIdAsync(score.UserId);

            //    if (user != null)
            //    {
            //        var model = new PDFModel()
            //        {
            //            Name = user.FirstName + " " + user.LastName,
            //            Score = score.HeinekenScore,
            //            testType = "Heineken"
            //        };
            //        scoreModel.Add(model);
            //    }
            //}
            //scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            if (scoreModel.Count > 10)
                scoreModel = scoreModel.Take(10).ToList();

            var report = scoreModel;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintScoreByAssessmentPDF(stream, report);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "Score by Heineken Assessment.pdf");
        }

        public async Task<IActionResult> ScoreByGuinnessAssessmentPDF()
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            //var scores = await _globalScoreRepository.GetListAsync();

            //foreach (var score in scores)
            //{
            //    var user = await _userManager.FindByIdAsync(score.UserId);

            //    if (user != null)
            //    {
            //        var model = new PDFModel()
            //        {
            //            Name = user.FirstName + " " + user.LastName,
            //            Score = score.GuinnessScore,
            //            testType = "Guinness"
            //        };
            //        scoreModel.Add(model);
            //    }
            //}
            //scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            if (scoreModel.Count > 10)
                scoreModel = scoreModel.Take(10).ToList();

            var report = scoreModel;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintScoreByAssessmentPDF(stream, report);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "Score by Guinness Assessment.pdf");
        }

        public async Task<IActionResult> ScoreByTheoryAssessmentPDF()
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            //var scores = await _globalScoreRepository.GetListAsync();

            //foreach (var score in scores)
            //{
            //    var user = await _userManager.FindByIdAsync(score.UserId);

            //    if (user != null)
            //    {
            //        var model = new PDFModel()
            //        {
            //            Name = user.FirstName + " " + user.LastName,
            //            Score = score.TheoryScore,
            //            testType = "Theory"
            //        };
            //        scoreModel.Add(model);
            //    }
            //}
            //scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            if (scoreModel.Count > 10)
                scoreModel = scoreModel.Take(10).ToList();

            var report = scoreModel;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintScoreByAssessmentPDF(stream, report);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "Score by Theory Assessment.pdf");
        }

        public async Task<IActionResult> ScoreByStoreHeinekenAssessmentPDF(int storeId)
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            var users = await _userExtensionRepository.GetListAsync();
            users = users.Where(x => x.Store == storeId).ToList();

            //var scores = await _globalScoreRepository.GetListAsync();

            foreach (var user in users)
            {
                //var score = await _globalScoreRepository.GetByUserIdAsync(user.UserId);

                //if (score != null)
                //{
                //    var model = new PDFModel()
                //    {
                //        Name = user.Name,
                //        Score = score.HeinekenScore,
                //        testType = "Heineken"
                //    };

                //    scoreModel.Add(model);
                //}
            }
            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            var report = scoreModel;

            if (report.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintScoreByStoreAssessmentPDF(stream, report);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/pdf", "Score by Store Heineken Assessment.pdf");
            }
            else
                _notify.Information($"There are no scores for this store");

            return RedirectToAction("Index", "Store", new { Area = "Store" });

        }

        public async Task<IActionResult> ScoreByStoreGuinnessAssessmentPDF(int storeId)
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            var users = await _userExtensionRepository.GetListAsync();
            users = users.Where(x => x.Store == storeId).ToList();

            //var scores = await _globalScoreRepository.GetListAsync();

            foreach (var user in users)
            {
                //var score = await _globalScoreRepository.GetByUserIdAsync(user.UserId);

                //if (score != null)
                //{
                //    var model = new PDFModel()
                //    {
                //        Name = user.Name,
                //        Score = score.GuinnessScore,
                //        testType = "Guinness"
                //    };

                //    scoreModel.Add(model);
                //}
            }
            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            var report = scoreModel;

            if (report.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintScoreByStoreAssessmentPDF(stream, report);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/pdf", "Score by Store Guinness Assessment.pdf");
            }
            else
                _notify.Information($"There are no scores for this store");


            return RedirectToAction("Index", "Store", new { Area = "Store" });
        }

        public async Task<IActionResult> ScoreByStoreTheoryAssessmentPDF(int storeId)
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            var users = await _userExtensionRepository.GetListAsync();
            users = users.Where(x => x.Store == storeId).ToList();

            //var scores = await _globalScoreRepository.GetListAsync();

            foreach (var user in users)
            {
                //var score = await _globalScoreRepository.GetByUserIdAsync(user.UserId);

                //if (score != null)
                //{
                //    var pdfModel = new PDFModel()
                //    {
                //        Name = user.Name,
                //        Score = score.TheoryScore,
                //        testType = "Theory"
                //    };

                //    scoreModel.Add(pdfModel);
                //}
            }
            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            var report = scoreModel;

            if (report.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintScoreByStoreAssessmentPDF(stream, report);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/pdf", "Score by Store Theory Assessment.pdf");
            }
            else
                _notify.Information($"There are no scores for this store");

            return RedirectToAction("Index", "Store", new { Area = "Store" });
        }

        public async Task<IActionResult> ScoreByStoreGlobalScorePDF(int storeId)
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            var users = await _userExtensionRepository.GetListAsync();
            users = users.Where(x => x.Store == storeId).ToList();

            //var scores = await _globalScoreRepository.GetListAsync();

            foreach (var user in users)
            {
                //var score = await _globalScoreRepository.GetByUserIdAsync(user.UserId);

                //if (score != null)
                //{
                //    var model = new PDFModel()
                //    {
                //        Name = user.Name,
                //        Score = score.TotalScore
                //    };

                //    scoreModel.Add(model);
                //}
            }
            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            var report = scoreModel;

            if (report.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintScoreByStoreAssessmentPDF(stream, report);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/pdf", "Score by Store All Assessments.pdf");
            }
            else
                _notify.Information($"There are no scores for this store");

            return RedirectToAction("Index", "Store", new { Area = "Store" });
        }

        public async Task<IActionResult> AllUsersPDF()
        {
            byte[] bytes;
            var scoreModel = new List<PDFModel>();
            var scores = await _userExtensionRepository.GetListAsync();

            foreach (var score in scores)
            {
                var store = await _storeRepository.GetByIdAsync(score.Store);

                var model = new PDFModel()
                {
                    Name = score.Name,
                    Outlet = score.Outlet,
                    State = store.State
                };

                scoreModel.Add(model);
            }
            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            var report = scoreModel;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintAllUsers(stream, report);
                bytes = stream.ToArray();
            }

            return File(bytes, "application/pdf", "All Users.pdf");
        }

        public async Task<IActionResult> AllBartendersPDF()
        {
            byte[] bytes;
            var bartenderList = new List<BartenderPDFModel>();
            var bartenders = await _userExtensionRepository.GetListAsync();

            foreach (var bartender in bartenders)
            {
                var model = new BartenderPDFModel()
                {
                    Name = bartender.Name,
                    IC = bartender.Ic,
                    PhoneNo = bartender.Telephone,
                    Gender = bartender.Gender,
                    OutletName = bartender.Outlet,
                    OutletAddress = bartender.OutletAddress,
                    OutletLocation = bartender.OutletLocation,
                    JoiningAs = bartender.JoiningAs,
                    Designation = bartender.Designation,
                    ShirtSize = bartender.UniformSize
                };

                bartenderList.Add(model);
            }
            bartenderList = bartenderList.OrderBy(x => x.Name).ToList();

            var report = bartenderList;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintAllBartenders(stream, report);
                bytes = stream.ToArray();
            }

            return File(bytes, "application/pdf", "All Bartenders.pdf");
        }

        public async Task<IActionResult> AllJudgesPDF()
        {
            byte[] bytes;
            var judgeList = new List<JudgePDFModel>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var userRole = await _userManager.GetRolesAsync(user);
                string roles = "";

                if (userRole.Contains("Guinness Judge") || userRole.Contains("Heineken Judge"))
                {
                    if (userRole.Contains("Guinness Judge"))
                        roles += "Guinness Judge";

                    if (userRole.Contains("Heineken Judge"))
                    {
                        if (roles != "")
                            roles += ", Heineken Judge";
                        else
                            roles += "Heineken Judge";
                    }

                    var judge = new JudgePDFModel()
                    {
                        Name = user.FirstName + " " + user.LastName,
                        Email = user.Email,
                        Role = roles
                    };

                    judgeList.Add(judge);
                }
            }
            judgeList = judgeList.OrderBy(x => x.Name).ToList();

            var report = judgeList;

            using (var stream = new MemoryStream())
            {
                await _pdfService.PrintAllJudges(stream, report);
                bytes = stream.ToArray();
            }

            return File(bytes, "application/pdf", "All Judges.pdf");
        }
    }
}