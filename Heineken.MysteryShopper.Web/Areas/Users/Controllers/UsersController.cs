using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyUs.Application.Features.UserExtension.Commands.Update;
using SurveyUs.Application.Features.UserExtension.Queries.GetAllCached;
using SurveyUs.Application.Features.UserExtension.Queries.GetById;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Users.Models;

namespace SurveyUs.Web.Areas.Users.Controllers
{
    [Area("Users")]
    public class UsersController : BaseController<UsersController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserExtensionRepository _userExtensionRepository;

        public UsersController(UserManager<ApplicationUser> userManager,
            IUserExtensionRepository userExtensionRepository)
        {
            _userManager = userManager;
            _userExtensionRepository = userExtensionRepository;
        }

        public IActionResult Index(int testId, int storeId, int userGroupId)
        {
            TempData["TEST_ID"] = testId;
            TempData["REGION_ID"] = storeId;
            TempData["USERGROUPID"] = userGroupId;

            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            //var userBartender = await _userManager.GetUsersInRoleAsync("Bartender");
            //var userModel = _mapper.Map<IEnumerable<UsersViewModel>>(userBartender);

            var filteredBartenders = new List<BartenderViewModel>();
            var bartendersResponse = await _mediator.Send(new GetAllUserExtensionCachedQuery());

            if (bartendersResponse.Succeeded)
            {
                var bartenders = _mapper.Map<List<BartenderViewModel>>(bartendersResponse.Data);
                var storeId = Convert.ToInt32(TempData.Peek("REGION_ID"));
                var userGroupId = Convert.ToInt32(TempData.Peek("USERGROUPID"));
                var testId = Convert.ToInt32(TempData.Peek("TEST_ID"));

                foreach (var bartender in bartenders)
                {
                    if (User.IsInRole("Guinness Judge") || User.IsInRole("Heineken Judge") && !(User.IsInRole("SuperAdmin") || User.IsInRole("Admin")))
                    {
                        if (bartender.Store == storeId && !bartender.IsDeleted && bartender.IsVerified)
                        {
                            if (userGroupId == 1)
                            {
                                if (bartender.JoiningAs.Contains("Bartendar"))
                                    filteredBartenders.Add(bartender);
                            }
                            if (userGroupId == 2)
                            {
                                if (bartender.JoiningAs.Contains("Heineken Distributor"))
                                    filteredBartenders.Add(bartender);
                            }
                            if (userGroupId == 3)
                            {
                                if (bartender.JoiningAs.Contains("Internal HMB Team"))
                                    filteredBartenders.Add(bartender);
                            }
                            if (userGroupId == 4)
                            {
                                if (bartender.JoiningAs.Contains("Outlet Representative"))
                                    filteredBartenders.Add(bartender);
                            }

                            bartender.TestId = Convert.ToInt32(TempData.Peek("TEST_ID"));
                        }
                    }
                    else
                    {
                        if (bartender.Store == storeId && !bartender.IsDeleted)
                        {
                            if (userGroupId == 1)
                            {
                                if (bartender.JoiningAs.Contains("Bartendar"))
                                    filteredBartenders.Add(bartender);
                            }
                            if (userGroupId == 2)
                            {
                                if (bartender.JoiningAs.Contains("Heineken Distributor"))
                                    filteredBartenders.Add(bartender);
                            }
                            if (userGroupId == 3)
                            {
                                if (bartender.JoiningAs.Contains("Internal HMB Team"))
                                    filteredBartenders.Add(bartender);
                            }
                            if (userGroupId == 4)
                            {
                                if (bartender.JoiningAs.Contains("Outlet Representative"))
                                    filteredBartenders.Add(bartender);
                            }

                            bartender.TestId = Convert.ToInt32(TempData.Peek("TEST_ID"));
                        }
                    }
                }

                if (testId == 1 || testId == 3)
                    filteredBartenders = filteredBartenders.OrderBy(x => x.Name).ToList();
                else
                    filteredBartenders = filteredBartenders.OrderByDescending(x => x.Name).ToList();

                //await ChangeVerificationStatus();

                return PartialView("_ViewAllUsers", filteredBartenders);
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> ChangeVerificationStatus(int testId, int userId, int isVerified)
        {
            TempData["USER_ID"] = userId;
            TempData["IS_VERIFIED"] = isVerified;
            TempData["TEST_ID"] = testId;

            var user = await _mediator.Send(new GetUserExtensionByIdQuery() { Id = Convert.ToInt32(TempData.Peek("USER_ID").ToString()) });
            var userIdentity = await _userManager.FindByIdAsync(user.Data.UserId);

            if (Convert.ToInt32(TempData.Peek("IS_VERIFIED").ToString()) == 1)
            {
                var bartenderVerified = new BartenderViewModel()
                {
                    Id = user.Data.Id,
                    UserId = user.Data.UserId,
                    Email = user.Data.Email,
                    Name = user.Data.Name,
                    Ic = user.Data.Ic,
                    Gender = user.Data.Gender,
                    Telephone = user.Data.Telephone,
                    Outlet = user.Data.Outlet,
                    OutletAddress = user.Data.OutletAddress,
                    UniformSize = user.Data.UniformSize,
                    Designation = user.Data.Designation,
                    IsVerified = false,
                    Store = user.Data.Store,
                    HeinekenTestStatus = user.Data.HeinekenTestStatus,
                    GuinnessTestStatus = user.Data.GuinnessTestStatus,
                    TheoryTestStatus = user.Data.TheoryTestStatus
                };

                var updateBartender = _mapper.Map<UpdateUserExtensionCommand>(bartenderVerified);
                var updateResult = await _mediator.Send(updateBartender);

                userIdentity.IsActive = false;
                await _userManager.UpdateAsync(userIdentity);
            }
            else
            {
                var bartenderUnverified = new BartenderViewModel()
                {
                    Id = user.Data.Id,
                    UserId = user.Data.UserId,
                    Email = user.Data.Email,
                    Name = user.Data.Name,
                    Ic = user.Data.Ic,
                    Gender = user.Data.Gender,
                    Telephone = user.Data.Telephone,
                    Outlet = user.Data.Outlet,
                    OutletAddress = user.Data.OutletAddress,
                    UniformSize = user.Data.UniformSize,
                    Designation = user.Data.Designation,
                    IsVerified = true,
                    Store = user.Data.Store,
                    HeinekenTestStatus = user.Data.HeinekenTestStatus,
                    GuinnessTestStatus = user.Data.GuinnessTestStatus,
                    TheoryTestStatus = user.Data.TheoryTestStatus
                };

                var updateBartender = _mapper.Map<UpdateUserExtensionCommand>(bartenderUnverified);
                var updateResult = await _mediator.Send(updateBartender);

                userIdentity.IsActive = true;
                await _userManager.UpdateAsync(userIdentity);
            }

            return RedirectToAction("Index", new { testId = TempData.Peek("TEST_ID"), storeId = TempData.Peek("REGION_ID"), userGroupId = TempData.Peek("USERGROUPID") });
        }

        public async Task<IActionResult> OnGetEdit(int id, int testId)
        {
            TempData["USER_ID"] = id;
            TempData["TEST_ID"] = testId;
            var response = await _mediator.Send(new GetUserExtensionByIdQuery() { Id = id });

            if (response.Succeeded)
            {
                var bartenderViewModel = _mapper.Map<BartenderViewModel>(response.Data);
                bartenderViewModel.Gender = bartenderViewModel.Gender.Trim();
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_EditBartender", bartenderViewModel) });
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostEdit(BartenderViewModel bartender, int userId)
        {
            var user = await _userExtensionRepository.GetByIdAsync(Convert.ToInt32(TempData["USER_ID"]));
            var userGroupId = Convert.ToInt32(TempData.Peek("USERGROUPID").ToString());
            var testId = Convert.ToInt32(TempData.Peek("TEST_ID"));

            //Adding Relevant fields to be updated
            bartender.Id = user.Id;
            bartender.UserId = user.UserId;
            bartender.Email = user.Email;
            bartender.Ic = user.Ic;
            bartender.Outlet = user.Outlet;
            bartender.OutletAddress = user.OutletAddress;
            bartender.OutletLocation = user.OutletLocation;
            bartender.JoiningAs = user.JoiningAs;
            bartender.Designation = user.Designation;
            bartender.IsVerified = user.IsVerified;
            bartender.IsDeleted = user.IsDeleted;
            bartender.Store = user.Store;
            bartender.HeinekenTestStatus = user.HeinekenTestStatus;
            bartender.GuinnessTestStatus = user.GuinnessTestStatus;
            bartender.TheoryTestStatus = user.TheoryTestStatus;

            if (ModelState.IsValid)
            {
                var updateBartenderCommand = _mapper.Map<UpdateUserExtensionCommand>(bartender);
                var result = await _mediator.Send(updateBartenderCommand);
                if (result.Succeeded)
                    _notify.Success($"Bartender Updated.");
            }

            var response = await _mediator.Send(new GetAllUserExtensionCachedQuery());
            if (response.Succeeded)
            {
                var storeId = Convert.ToInt32(TempData.Peek("REGION_ID"));
                var viewModel = _mapper.Map<List<BartenderViewModel>>(response.Data);
                var bartenderList = new List<BartenderViewModel>();

                foreach (var model in viewModel)
                {
                    var bartenderViewModel = new BartenderViewModel()
                    {
                        Id = model.Id,
                        UserId = model.UserId,
                        Email = model.Email,
                        Name = model.Name,
                        Ic = model.Ic,
                        Gender = model.Gender,
                        Telephone = model.Telephone,
                        Outlet = model.Outlet,
                        OutletAddress = model.OutletAddress,
                        UniformSize = model.UniformSize,
                        Designation = model.Designation,
                        JoiningAs = model.JoiningAs,
                        IsVerified = model.IsVerified,
                        Store = model.Store,
                        HeinekenTestStatus = model.HeinekenTestStatus,
                        GuinnessTestStatus = model.GuinnessTestStatus,
                        TheoryTestStatus = model.TheoryTestStatus,
                        TestId = testId
                    };

                    if (model.Store == storeId && !model.IsDeleted)
                        bartenderList.Add(bartenderViewModel);
                }

                if (userGroupId == 1)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Bartendar")).ToList();

                if (userGroupId == 2)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Heineken Distributor")).ToList();

                if (userGroupId == 3)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Internal HMB Team")).ToList();

                if (userGroupId == 4)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Outlet Representative")).ToList();

                if (testId == 1 || testId == 3)
                    bartenderList = bartenderList.OrderBy(x => x.Name).ToList();
                else
                    bartenderList = bartenderList.OrderByDescending(x => x.Name).ToList();

                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAllUsers", bartenderList);
                return new JsonResult(new { isValid = true, html, userGroupId = TempData.Peek("USERGROUPID"), storeId = TempData.Peek("REGION_ID"), testId = TempData.Peek("TEST_ID") });
            }

            return default;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostDelete(BartenderViewModel bartender, int testId, int userId, int storeId)
        {
            var userResponse = await _mediator.Send(new GetUserExtensionByIdQuery() { Id = userId });
            var userGroupId = Convert.ToInt32(TempData.Peek("USERGROUPID").ToString());

            if (userResponse.Succeeded)
            {
                bartender = _mapper.Map<BartenderViewModel>(userResponse.Data);
            }

            var user = await _userExtensionRepository.GetByIdAsync(userId);
            bartender.Id = userId;
            bartender.UserId = user.UserId;
            bartender.IsVerified = user.IsVerified;
            bartender.IsDeleted = true;

            //var scoreResponse = await _mediator.Send(new GetAllGlobalScoresCachedQuery());

            //if (scoreResponse.Succeeded)
            //{
            //    var globalScoreRepo = await _globalScoreRepository.GetByUserIdAsync(user.UserId);

            //    // Set IsDeleted for Global Score
            //    if (globalScoreRepo != null)
            //    {
            //        var globalScore = new GlobalScoreModel()
            //        {
            //            Id = globalScoreRepo.Id,
            //            UserId = userResponse.Data.UserId,
            //            HeinekenScore = globalScoreRepo.HeinekenScore,
            //            GuinnessScore = globalScoreRepo.GuinnessScore,
            //            TheoryScore = globalScoreRepo.TheoryScore,
            //            TotalScore = globalScoreRepo.TotalScore,
            //            IsDeleted = true
            //        };

            //        var updateGlobalScoreCommand = _mapper.Map<UpdateGlobalScoreCommand>(globalScore);
            //        var updateResult = await _mediator.Send(updateGlobalScoreCommand);
            //    }
            //}

            var userIdentity = await _userManager.FindByIdAsync(user.UserId);

            //var Identity = new ApplicationUser
            //{
            //    UserName = userIdentity.UserName,
            //    Email = userIdentity.Email,
            //    FirstName = userIdentity.FirstName,
            //    LastName = userIdentity.LastName,
            //    EmailConfirmed = true
            //};

            //var resulta = await _userManager.UpdateAsync(Identity);

            var guidUsername = await _userManager.SetUserNameAsync(userIdentity, user.UserId + "_" + userIdentity.UserName);

            if (ModelState.IsValid)
            {
                var updateBaretenderCommand = _mapper.Map<UpdateUserExtensionCommand>(bartender);
                var result = await _mediator.Send(updateBaretenderCommand);
                if (result.Succeeded)
                    _notify.Success($"Bartender Deleted.");
            }

            var response = await _mediator.Send(new GetAllUserExtensionCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<BartenderViewModel>>(response.Data);
                var bartenderList = new List<BartenderViewModel>();

                foreach (var model in viewModel)
                {
                    if (!model.IsDeleted)
                    {
                        var bartenderViewModel = new BartenderViewModel()
                        {
                            Id = model.Id,
                            UserId = model.UserId,
                            Email = model.Email,
                            Name = model.Name,
                            Ic = model.Ic,
                            Gender = model.Gender,
                            Telephone = model.Telephone,
                            Outlet = model.Outlet,
                            OutletAddress = model.OutletAddress,
                            UniformSize = model.UniformSize,
                            Designation = model.Designation,
                            JoiningAs = model.JoiningAs,
                            IsVerified = model.IsVerified,
                            Store = model.Store,
                            HeinekenTestStatus = model.HeinekenTestStatus,
                            GuinnessTestStatus = model.GuinnessTestStatus,
                            TheoryTestStatus = model.TheoryTestStatus,
                            TestId = testId
                        };

                        if (model.Store == storeId && !model.IsDeleted)
                            bartenderList.Add(bartenderViewModel);
                    }
                }

                if (userGroupId == 1)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Bartendar")).ToList();

                if (userGroupId == 2)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Heineken Distributor")).ToList();

                if (userGroupId == 3)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Internal HMB Team")).ToList();

                if (userGroupId == 4)
                    bartenderList = bartenderList.Where(x => x.JoiningAs.Contains("Outlet Representative")).ToList();


                if (testId == 1 || testId == 3)
                    bartenderList = bartenderList.OrderBy(x => x.Name).ToList();
                else
                    bartenderList = bartenderList.OrderByDescending(x => x.Name).ToList();

                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAllUsers", bartenderList);
                return new JsonResult(new { isValid = true, html });
            }

            return null;
        }
    }
}
