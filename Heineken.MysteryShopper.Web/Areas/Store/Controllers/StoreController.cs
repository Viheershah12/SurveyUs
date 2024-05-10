using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Application.Features.Store.Commands.Create;
using SurveyUs.Application.Features.Store.Commands.Delete;
using SurveyUs.Application.Features.Store.Commands.Update;
using SurveyUs.Application.Features.Store.Queries.GetAllCached;
using SurveyUs.Application.Features.Store.Queries.GetById;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Areas.Store.Models;

namespace SurveyUs.Web.Areas.Store.Controllers
{
    [Area("Store")]
    public class StoreController : BaseController<StoreController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStoreRepository _storeRepository;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;
        private readonly IStoreMappingsRepository _storeMappingsRepository;
        private readonly IUserAnswersRepository _userAnswersRepository;

        public StoreController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IStoreRepository storeRepository,
            ICampaignMappingsRepository campaignMappingsRepository,
            IStoreMappingsRepository storeMappingsRepository,
            IUserAnswersRepository userAnswersRepository
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _storeRepository = storeRepository;
            _campaignMappingsRepository = campaignMappingsRepository;
            _storeMappingsRepository = storeMappingsRepository;
            _userAnswersRepository = userAnswersRepository;
        }

        public async Task<IActionResult> Index(int campaignId)
        {
            var campaignDetail = await _mediator.Send(new GetCampaignByIdQuery() { Id = campaignId });

            var campaign = new ScoreboardStoreViewModel
            {
                CampaignId = campaignId,
                CampaignName = campaignDetail.Data.Name
            };

            return View(campaign);
        }

        //public IActionResult UserGroups(int storeId)
        //{
        //    TempData["REGION_ID"] = storeId;
        //    return View("UserGroups");
        //}

        //public IActionResult Tests(int storeId, int userGroupId)
        //{
        //    TempData["REGION_ID"] = storeId;
        //    TempData["USERGROUPID"] = userGroupId;
        //    return View("Tests");
        //}

        public IActionResult Campaigns(int storeId)
        {
            ViewBag.StoreId = storeId;
            return View("Campaigns");
        }

        public async Task<IActionResult> LoadAll(int campaignId)
        {
            if (User.IsInRole("Mystery Drinker"))
            {
                return RedirectToAction("LoadAllForMysteryShopper", new { campaignId = campaignId });
            }
            var storeIds = _campaignMappingsRepository.CampaignMappings.Where(m => m.CampaignId == campaignId).Select(m => m.StoreId).ToList();

            // Check for stores which quiz is already answered
            var quizCompletedStores = _userAnswersRepository.UserAnswers.Where(a => storeIds.Contains(a.StoreId) && a.CampaignId == campaignId).Select(a => a.StoreId).Distinct().ToList();
            List<int> disabledStores = storeIds.Intersect(quizCompletedStores).ToList();

            var stores = _storeRepository.Store.Where(s => storeIds.Contains(s.Id)).ToList();
            List<StoreViewModel> storeList = new List<StoreViewModel>();

            if (stores.Count != 0)
            {
                storeList = _mapper.Map<List<StoreViewModel>>(stores);

                // mark stores with completed quiz
                if (disabledStores.Count != 0)
                {
                    foreach (var store in storeList)
                    {
                        if (disabledStores.Contains(store.Id))
                        {
                            store.IsCompletedQuiz = true;
                        }
                    }
                }
            }

            ScoreboardStoreViewModel viewModel = new ScoreboardStoreViewModel
            {
                Stores = storeList,
                CampaignId = campaignId
            };

            return PartialView("_ViewAll", viewModel);
        }

        public async Task<IActionResult> LoadAllForMysteryShopper(int campaignId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Find stores assigned to user
            var storeIdsAssignedToUser = _storeMappingsRepository.StoreMappings.Where(s => s.UserId == userId).Select(s => s.StoreId).ToList();

            // Filter the stores (to only show those which are assigned to campaignId)
            var storeIds = _campaignMappingsRepository.CampaignMappings.Where(m => storeIdsAssignedToUser.Contains(m.StoreId) && m.CampaignId == campaignId).Select(m => m.StoreId).ToList();

            // Check for stores which quiz is already answered
            var quizCompletedStores = _userAnswersRepository.UserAnswers.Where(a => storeIds.Contains(a.StoreId) && a.CampaignId == campaignId && a.UserId == userId).Select(a => a.StoreId).Distinct().ToList();
            List<int> disabledStores = storeIds.Intersect(quizCompletedStores).ToList();

            // Get store details
            var stores = _storeRepository.Store.Where(s => storeIds.Contains(s.Id)).ToList();

            List<StoreViewModel> storeList = new List<StoreViewModel>();

            if (stores.Count != 0)
            {
                storeList = _mapper.Map<List<StoreViewModel>>(stores);

                // mark stores with completed quiz
                if (disabledStores.Count != 0)
                {
                    foreach (var store in storeList)
                    {
                        if (disabledStores.Contains(store.Id))
                        {
                            store.IsCompletedQuiz = true;
                        }
                    }
                }
            }

            ScoreboardStoreViewModel viewModel = new ScoreboardStoreViewModel
            {
                Stores = storeList,
                CampaignId = campaignId
            };

            return PartialView("_ViewAll", viewModel);
        }

        //public async Task<IActionResult> LoadAllUserGroups()
        //{
        //    return PartialView("_ViewAllUserGroups", TempData.Peek("REGIONID"));
        //}

        //public async Task<IActionResult> LoadAllTests()
        //{
        //    return PartialView("_ViewAllTests", TempData.Peek("REGION_ID"));
        //}

        public async Task<IActionResult> LoadAllCampaigns()
        {
            //var campaignsResponse = await _mediator.Send(new GetCampaignsById { Id = ViewBag.StoreId });
            return PartialView("_ViewAllCampaigns");
        }

        public async Task<IActionResult> OnGetCreate()
        {
            var storesViewModel = new StoreViewModel();
            storesViewModel.CreatedOn = DateTime.Now;
            storesViewModel.UpdatedOn = DateTime.Now;
            var stateList = EnumHelper.ToSelectionList<StateEnum>();
            foreach (var state in stateList)
            {
                storesViewModel.StateDropdown.Add(new SelectListItem()
                {
                    Text = state.Name,
                    Value = state.Id.ToString(),
                });
            }
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", storesViewModel) });
        }

        public async Task<IActionResult> OnGetEdit(int id)
        {
            TempData["REGION_ID"] = id;
            var response = await _mediator.Send(new GetStoreByIdQuery() { Id = id });

            if (response.Succeeded)
            {
                var storesViewModel = _mapper.Map<StoreViewModel>(response.Data);
                storesViewModel.UpdatedOn = DateTime.Now;
                var stateList = EnumHelper.ToSelectionList<StateEnum>();
                foreach (var state in stateList)
                {
                    storesViewModel.StateDropdown.Add(new SelectListItem()
                    {
                        Text = state.Name,
                        Value = state.Id.ToString(),
                    });
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Edit", storesViewModel) });
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreate(StoreViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createStoreCommand = _mapper.Map<CreateStoreCommand>(userModel);
                    var result = await _mediator.Send(createStoreCommand);
                    if (result.Succeeded)
                    {
                        var id = userModel.Id;
                        _notify.Success($"Store with ID {id} Created.");
                    }
                }
                catch (Exception ex) 
                {

                }
            }

            var response = await _mediator.Send(new GetAllStoresCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<StoreViewModel>>(response.Data);
                var storesList = new List<StoreViewModel>();

                foreach (var model in viewModel)
                {
                    var storesViewModel = new StoreViewModel()
                    {
                        Id = model.Id,
                        State = model.State,
                        Name = model.Name,
                        CreatedOn = model.CreatedOn,
                        IsActive = model.IsActive,
                    };
                    storesList.Add(storesViewModel);
                }

                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", storesList);
                return new JsonResult(new { isValid = true, html });
            }

            return default;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostEdit(StoreViewModel store, int storeId)
        {
            store.Id = storeId;

            if (ModelState.IsValid)
            {
                var updateStoresCommand = _mapper.Map<UpdateStoreCommand>(store);
                var result = await _mediator.Send(updateStoresCommand);
                if (result.Succeeded)
                    _notify.Information($"Store edited.");
            }

            var response = await _mediator.Send(new GetAllStoresCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<StoreViewModel>>(response.Data);
                var storesList = new List<StoreViewModel>();

                foreach (var model in viewModel)
                {
                    var storesViewModel = new StoreViewModel()
                    {
                        Id = model.Id,
                        State = model.State,
                        Name = model.Name,
                        CreatedOn = model.CreatedOn,
                        UpdatedOn = model.UpdatedOn,
                        IsActive = model.IsActive
                    };
                    storesList.Add(storesViewModel);
                }

                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", storesList);
                return new JsonResult(new { isValid = true, html });
            }

            return default;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteCommand = await _mediator.Send(new DeleteStoreCommand { Id = id });

            if (deleteCommand.Succeeded)
            {
                _notify.Information($"Store deleted.");
                var response = await _mediator.Send(new GetAllStoresCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<StoreViewModel>>(response.Data);
                    var storesList = new List<StoreViewModel>();

                    foreach (var model in viewModel)
                    {
                        if (!model.IsDeleted)
                        {
                            var storesViewModel = new StoreViewModel()
                            {
                                Id = model.Id,
                                State = model.State,
                                Name = model.Name,
                                IsActive = model.IsActive,
                                IsDeleted = model.IsDeleted
                            };
                            storesList.Add(storesViewModel);
                        }
                    }
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", storesList);
                    return new JsonResult(new { isValid = true, html });
                }
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostActivate(int storeId, bool isActivate)
        {
            var store = await _mediator.Send(new GetStoreByIdQuery() { Id = storeId });

            var storeViewModel = new StoreViewModel()
            {
                Id = store.Data.Id,
                State = store.Data.State,
                Name = store.Data.Name,
                Line1 = store.Data.Line1,
                Line2 = store.Data.Line2,
                CreatedOn = store.Data.CreatedOn,
                UpdatedOn = store.Data.UpdatedOn,
                IsActive = !isActivate,
                IsDeleted = store.Data.IsDeleted
            };
            try
            {
                var updateStoresCommand = _mapper.Map<UpdateStoreCommand>(storeViewModel);
                var result = await _mediator.Send(updateStoresCommand);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

            }

            return default;
        }
    }
}
