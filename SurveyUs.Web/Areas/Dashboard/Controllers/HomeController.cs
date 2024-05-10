using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Dashboard.Models;

namespace SurveyUs.Web.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : BaseController<HomeController>
    {
        private readonly IUserExtensionRepository _userExtensionRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IUserExtensionRepository userExtensionRepository,
            IStoreRepository storeRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            _userExtensionRepository = userExtensionRepository;
            _storeRepository = storeRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //_notify.Information("Hi There!");
            if (User.IsInRole("Bartender"))
                return LocalRedirect("~/test/theorytest");
            else if (User.IsInRole("Heineken Judge") || User.IsInRole("Guinness Judge"))
                return LocalRedirect("~/store/store");
            else if (User.IsInRole("MysteryShopper"))
                return LocalRedirect("~/campaign/campaign");

            var storeOptionModel = new List<StoreOption>();
            var scoreModel = new List<ScoreModel>();

            var stores = _storeRepository.GetListAsync();
            if (stores.Result.Count() != storeOptionModel.Count())
            {
                foreach (var state in stores.Result)
                {
                    var stateModel = new StoreOption()
                    {
                        State = state.State
                    };
                    storeOptionModel.Add(stateModel);
                }
            }

            if (stores.Result.Count() != storeOptionModel.Count())
            {
                foreach (var state in stores.Result)
                {
                    var stateModel = new StoreOption()
                    {
                        State = state.State
                    };
                    storeOptionModel.Add(stateModel);
                }
            }

            var model = new ScoreModel()
            {
                StoreOptions = storeOptionModel
            };
            scoreModel.Add(model);

            return View(scoreModel);
        }

        public async Task<IActionResult> GlobalScore()
        {
            var scoreModel = new List<ScoreModel>();
            var storeOptionModel = new List<StoreOption>();

            //if (globalScores != null)
            //{
            //    foreach (var score in globalScores)
            //    {
            //        if (!score.IsDeleted)
            //        {
            //            var user = await _userExtensionRepository.GetByUserIdAsync(score.UserId);
            //            var store = await _storeRepository.GetByIdAsync(user.Store);

            //            var stores = await _storeRepository.GetListAsync();
            //            if (stores.Count() != storeOptionModel.Count())
            //            {
            //                foreach (var state in stores)
            //                {
            //                    var stateModel = new StoreOption()
            //                    {
            //                        State = state.State
            //                    };
            //                    storeOptionModel.Add(stateModel);
            //                }
            //            }

            //            var model = new ScoreModel()
            //            {
            //                Name = user.Name,
            //                State = store.State,
            //                Score = score.TotalScore,
            //                RepresentAs = user.JoiningAs,
            //                OutletName = user.Outlet,
            //                StoreOptions = storeOptionModel
            //            };
            //            scoreModel.Add(model);
            //        }

            //    }
            //}

            if (scoreModel.Count() == 0)
            {
                var stores = await _storeRepository.GetListAsync();
                if (stores.Count() != storeOptionModel.Count())
                {
                    foreach (var state in stores)
                    {
                        var stateModel = new StoreOption()
                        {
                            State = state.State
                        };
                        storeOptionModel.Add(stateModel);
                    }
                }

                var model = new ScoreModel()
                {
                    StoreOptions = storeOptionModel
                };
                scoreModel.Add(model);
            }

            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            return PartialView("_ViewGlobalScores", scoreModel);
        }

        public async Task<IActionResult> AverageScoreByStore()
        {
            var scoreModel = new List<ScoreModel>();
            var stores = await _storeRepository.GetListAsync();

            foreach (var store in stores)
            {
                int totalScore = 0;
                var bartenders = await _userExtensionRepository.GetByStoreAsync(store.Id);

                foreach (var bartender in bartenders)
                {
                    //var score = await _globalScoreRepository.GetByUserIdAsync(bartender.UserId);

                    //if (score != null && !score.IsDeleted)
                    //    totalScore += score.TotalScore;
                }

                var model = new ScoreModel()
                {
                    State = store.State,
                    Score = totalScore / (bartenders.Count == 0 ? 1 : bartenders.Count),
                    BartenderCount = bartenders.Where(x => x.IsVerified).Count()
                };

                scoreModel.Add(model);
            }
            scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            return PartialView("_ViewAverageScoreByStore", scoreModel);
        }

        public async Task<IActionResult> ScoreByHeinekenAssessment()
        {
            var scoreModel = new List<ScoreModel>();
            //var scores = await _globalScoreRepository.GetListAsync();

            //foreach (var score in scores)
            //{
            //    if (!score.IsDeleted)
            //    {
            //        var user = await _userManager.FindByIdAsync(score.UserId);
            //        var userExtension = await _userExtensionRepository.GetByUserIdAsync(score.UserId);
            //        var store = await _storeRepository.GetByIdAsync(userExtension.Store);

            //        if (user != null)
            //        {
            //            var model = new ScoreModel()
            //            {
            //                Name = user.FirstName + " " + user.LastName,
            //                State = store.State,
            //                Score = score.HeinekenScore,
            //                Id = 1
            //            };
            //            scoreModel.Add(model);
            //        }
            //    }
            //}
            //scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            //if (scoreModel.Count > 10)
            //    scoreModel = scoreModel.Take(10).ToList();

            return PartialView("_ViewScoreByAssessment", scoreModel);
        }

        public async Task<IActionResult> ScoreByGuinnessAssessment()
        {
            var scoreModel = new List<ScoreModel>();
            //var scores = await _globalScoreRepository.GetListAsync();

            //foreach (var score in scores)
            //{
            //    if (!score.IsDeleted)
            //    {
            //        var user = await _userManager.FindByIdAsync(score.UserId);
            //        var userExtension = await _userExtensionRepository.GetByUserIdAsync(score.UserId);
            //        var store = await _storeRepository.GetByIdAsync(userExtension.Store);

            //        if (user != null)
            //        {
            //            var model = new ScoreModel()
            //            {
            //                Name = user.FirstName + " " + user.LastName,
            //                State = store.State,
            //                Score = score.GuinnessScore,
            //                Id = 2
            //            };
            //            scoreModel.Add(model);
            //        }
            //    }
            //}
            //scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            //if (scoreModel.Count > 10)
            //    scoreModel = scoreModel.Take(10).ToList();

            return PartialView("_ViewScoreByAssessment", scoreModel);
        }

        public async Task<IActionResult> ScoreByTheoryAssessment()
        {
            var scoreModel = new List<ScoreModel>();
            //var scores = await _globalScoreRepository.GetListAsync();

            //foreach (var score in scores)
            //{
            //    if (!score.IsDeleted)
            //    {
            //        var user = await _userManager.FindByIdAsync(score.UserId);
            //        var userExtension = await _userExtensionRepository.GetByUserIdAsync(score.UserId);
            //        var store = await _storeRepository.GetByIdAsync(userExtension.Store);

            //        if (user != null)
            //        {
            //            var model = new ScoreModel()
            //            {
            //                Name = user.FirstName + " " + user.LastName,
            //                State = store.State,
            //                Score = score.TheoryScore,
            //                Id = 3
            //            };
            //            scoreModel.Add(model);
            //        }
            //    }
            //}
            //scoreModel = scoreModel.OrderByDescending(x => x.Score).ToList();

            //if (scoreModel.Count > 10)
            //    scoreModel = scoreModel.Take(10).ToList();

            return PartialView("_ViewScoreByAssessment", scoreModel);
        }
    }
}