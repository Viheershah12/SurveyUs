using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyUs.Application.Features.Store.Commands.Create;
using SurveyUs.Application.Features.Store.Commands.Update;
using SurveyUs.Application.Features.Store.Queries.GetAllCached;
using SurveyUs.Application.Features.Store.Queries.GetById;
using SurveyUs.Application.Features.StoreMappings.Commands.Create;
using SurveyUs.Application.Features.StoreMappings.Commands.Update;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreSettingController : BaseController<StoreSettingController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserExtensionRepository _userExtensionRepository;
        private readonly IStoreMappingsRepository _storeMappingsRepository;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;
        private readonly IConfiguration _config;

        private const string _storeDataTable = "storesTable";

        public StoreSettingController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserExtensionRepository userExtensionRepository,
            IStoreMappingsRepository storeMappingsRepository,
            ICampaignMappingsRepository campaignMappingsRepository,
            IConfiguration config
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userExtensionRepository = userExtensionRepository;
            _storeMappingsRepository = storeMappingsRepository;
            _campaignMappingsRepository = campaignMappingsRepository;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetStoreListing(DataTableExtensions request)
        {
            var con = _config.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));

            var storeList = new List<StoreViewModel>();

            var skipCount = (pageNumber - 1) * request.length;

            var searchValue = request.search.value?.ToLower().Trim();
            var sortColumn = request.order[0]?.column;
            string sortColumnDirection = request.order[0]?.dir;
            string? sortColumnName = null;

            if (sortColumn != null && !string.IsNullOrEmpty(sortColumnDirection))
            {
                sortColumnName = request.columns[(int)sortColumn].name.ToLower();
            }

            var paramName = new List<string>();
            var paramValue = new List<object>();

            #region Raw Query 
            var rawQuery = @"
                    SELECT s.Id as ""StoreId"", s.Name, s.State, s.CreatedOn, s.IsActive, sta.Name
                    FROM dbo.Store s
                    LEFT JOIN dbo.State sta on sta.Id = s.State ";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"
                    WHERE LOWER(s.Name) LIKE '%' + @searchTerm + '%' 
                    OR LOWER(sta.Name) LIKE '%' + @searchTerm + '%' ";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "name")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"s.Name ";
                }
                else
                    rawQuery += @"s.Name DESC ";
            }
            else if (sortColumnName == "state")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"sta.Name ";
                }
                else
                    rawQuery += @"sta.Name DESC ";
            }
            else if (sortColumnName == "isactive")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"s.IsActive ";
                }
                else
                    rawQuery += @"s.IsActive DESC ";
            }
            else
            {
                rawQuery += "s.CreatedOn DESC ";
            }
            #endregion

            var selectQuery = new StringBuilder();
            selectQuery.Append(rawQuery);

            // create connection object first
            using (var cn = new SqlConnection(con))
            {
                var (totalCount, dt) = await QueryRepositoryExtensions.SelectDataTableWithCountAndPagination(cn, rawQuery.ToString(), paramName.ToArray(), paramValue.ToArray(), skipCount, request.length);

                using (dt)
                {
                    for (int i = 0; i < dt?.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];

                        if (dr == null)
                            continue;

                        var res = new StoreViewModel()
                        {
                            Id = dr["StoreId"] is DBNull ? 0 : Convert.ToInt32(dr["StoreId"]),
                            Name = dr["Name"] is DBNull ? string.Empty : dr["Name"].ToString(),
                            State = dr["State"] is DBNull ? 0 : (StateEnum)dr["State"],
                            CreatedOn = dr["CreatedOn"] is DBNull ? DateTime.Now : (DateTime)dr["CreatedOn"],
                            IsActive = dr["IsActive"] is DBNull ? false : Convert.ToBoolean(dr["IsActive"])
                        };

                        storeList.Add(res);
                    }
                }

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = storeList,
                });
            }
        }

        public async Task<IActionResult> OnGetCreate()
        {
            var storesViewModel = new StoreViewModel
            {
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                DataTableId = _storeDataTable
            };

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

        [HttpPost]
        public async Task<IActionResult> OnPostCreate(StoreViewModel store)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            store.CreatedBy = currentUser.Id;
            store.UpdatedBy = currentUser.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    var createStoreCommand = _mapper.Map<CreateStoreCommand>(store);
                    var result = await _mediator.Send(createStoreCommand);
                    if (result.Succeeded)
                    {
                        var id = store.Id;
                        _notify.Success($"Store '{store.Name}' Created.");
                        return new JsonResult(new { isValid = true, dataTableId = _storeDataTable });
                    }
                    _notify.Error($"Failed To Create Store '{store.Name}'.");
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { isValid = false });
                }
            }

            return new JsonResult(new { isValid = false });
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
        public async Task<IActionResult> OnPostEdit(StoreViewModel store, int storeId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            store.Id = storeId;
            store.UpdatedBy = currentUser.Id;

            if (ModelState.IsValid)
            {
                var updateStoresCommand = _mapper.Map<UpdateStoreCommand>(store);
                var result = await _mediator.Send(updateStoresCommand);
                if (result.Succeeded)
                {
                    _notify.Information($"Store '{store.Name}' Edited.");
                    return new JsonResult(new { isValid = true, dataTableId = _storeDataTable });
                }
                _notify.Error($"Failed To Edit Store '{store.Name}'.");
            }
            return new JsonResult(new { isValid = false });
        }

        public async Task<IActionResult> OnGetAssign(int id)
        {
            var usersList = new ShopperMappingViewModel();
            var userOptionsList = new List<UserViewModel>();
            var storeMappingsUsers = await _storeMappingsRepository.StoreMappings.Where(u => u.StoreId == id && u.IsDeleted == false).ToListAsync();
            var storeMappingsUserIds = storeMappingsUsers.Select(u => u.UserId).ToList();
            
            var mysteryShopperUsers = await _userManager.GetUsersInRoleAsync("Mystery Drinker");
            
            var usersNotInStoreMappings = mysteryShopperUsers
                .Where(u => !storeMappingsUserIds.Contains(u.Id))
                .ToList();
            //var usersToAdd = mysteryShopperUsersIds.Except(storeMappingUsersIds);
            //var usersToAddObjects = mysteryShopperUsers.Where(u => usersToAdd.Contains(u.Id)).ToList();

            foreach (var user in usersNotInStoreMappings)
            {
                if (user.IsActive)
                {
                    var userRole = await _userManager.GetRolesAsync(user);
                    string roles = "";

                    if (userRole != null)
                    {
                        for (int i = 0; i < userRole.Count; i++)
                        {
                            if (i == userRole.Count - 1)
                                roles += userRole[i];
                            else
                                roles += userRole[i] + ", ";
                        }
                    }

                    var userOptions = new UserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName?.ToUpper(),
                        LastName = user.LastName?.ToUpper(),
                        Email = user.Email,
                        IsActive = user.IsActive,
                        Role = roles
                    };

                    userOptionsList.Add(userOptions);
                }             
            }
            usersList = new ShopperMappingViewModel
            {
                StoreId = id,
                MShopperMapping = userOptionsList
            };
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Assign", usersList) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAssign(string userIds, int storeId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            string[] userIdArray = userIds.Split(',');
            Result<int> result = new Result<int>();
            var store = await _mediator.Send(new GetStoreByIdQuery() { Id = storeId });
            foreach (var userId in userIdArray)
            {
                try
                {
                    //var user = await _userExtensionRepository.GetByUserIdAsync(userId);
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var storeMappingRow = _storeMappingsRepository.StoreMappings.Where(u => u.UserId == user.Id && u.StoreId == storeId).FirstOrDefaultAsync();
                        //Update Existing StoreMapping
                        if (storeMappingRow.Result != null)
                        {
                            StoreMappingsViewModel storeMapping = new StoreMappingsViewModel()
                            {
                                Id = storeMappingRow.Result.Id,
                                UserId = user.Id,
                                StoreId = storeId,
                                IsDeleted = false,
                                UpdatedBy = currentUser.Id
                            };

                            var updateStoreMappingsCommand = _mapper.Map<UpdateStoreMappingsCommand>(storeMapping);
                            result = await _mediator.Send(updateStoreMappingsCommand);
                        }
                        //Create new StoreMapping   
                        else
                        {
                            StoreMappingsViewModel storeMapping = new StoreMappingsViewModel()
                            {
                                UserId = user.Id,
                                StoreId = storeId,
                                IsDeleted = false,
                                CreatedBy = currentUser.Id,
                                UpdatedBy = currentUser.Id
                            };

                            var createStoreMappingsCommand = _mapper.Map<CreateStoreMappingsCommand>(storeMapping);
                            result = await _mediator.Send(createStoreMappingsCommand);
                            
                        }
                    }
                }
                catch (Exception ex)
                { 
                }
            }

            if (result.Succeeded)
            {
                _notify.Information($"User(s) Assigned to {store.Data.Name}.");
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
                        Name = model?.Name,
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
        public async Task<IActionResult> OnPostActivate(int storeId, string storeName, bool isActivate)
        {
            var storeViewModel = new StoreViewModel()
            {
                Id = storeId,
                IsActive = !isActivate,
            };

            try
            {
                var updateStoresCommand = _mapper.Map<UpdateStoreCommand>(storeViewModel);
                var result = await _mediator.Send(updateStoresCommand);

                if (result.Succeeded)
                {
                    if(!isActivate)
                    {
                        _notify.Information($"Store '{storeName}' Activated.");
                    }
                    else
                    {
                        _notify.Information($"Store '{storeName}' Deactivated.");
                    }
                    return new JsonResult(new { isValid = true, dataTableId = _storeDataTable });
                }
                _notify.Error($"Failed To Activate Store '{storeName}'.");
            }
            catch (Exception ex)
            {
                return new JsonResult(new { isValid = false });
            }

            return new JsonResult(new { isValid = false });
        }

        #region Unused
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

        //public async Task<IActionResult> LoadAll()
        //{
        //    var storesResponse = await _mediator.Send(new GetAllStoresCachedQuery());
        //    if (storesResponse.Succeeded)
        //    {
        //        try
        //        {
        //            var viewModel = _mapper.Map<List<StoreViewModel>>(storesResponse.Data);
        //            return PartialView("_ViewAll", viewModel);
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    return null;
        //}

        //[HttpPost]
        //public async Task<IActionResult> OnPostDelete(int id)
        //{
        //    var deleteCommand = await _mediator.Send(new DeleteStoreCommand { Id = id });

        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"Store deleted.");
        //        var response = await _mediator.Send(new GetAllStoresCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            //var viewModel = _mapper.Map<List<StoreViewModel>>(response.Data);
        //            //var storesList = new List<StoreViewModel>();

        //            //foreach (var model in viewModel)
        //            //{
        //            //    if (!model.IsDeleted)
        //            //    {
        //            //        var storesViewModel = new StoreViewModel()
        //            //        {
        //            //            Id = model.Id,
        //            //            State = model.State,
        //            //            Name = model?.Name,
        //            //            IsActive = model.IsActive,
        //            //            IsDeleted = model.IsDeleted
        //            //        };
        //            //        storesList.Add(storesViewModel);
        //            //    }
        //            //}
        //            //var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", storesList);
        //            _notify.Success("")
        //            return new JsonResult(new { isValid = true, html });
        //        }
        //    }

        //    return null;
        //}

        //public async Task<IActionResult> LoadAllUserGroups()
        //{
        //    return PartialView("_ViewAllUserGroups", TempData.Peek("REGIONID"));
        //}

        //public async Task<IActionResult> LoadAllTests()
        //{
        //    return PartialView("_ViewAllTests", TempData.Peek("REGION_ID"));
        //}

        //public async Task<IActionResult> OnGetAssignCampaigns(int storeId)
        //{
        //    var campaignsResponse = await _mediator.Send(new GetAllCampaignsCachedQuery());

        //    if (campaignsResponse.Succeeded)
        //    {
        //        var campaignViewModel = _mapper.Map<List<CampaignDetail>>(campaignsResponse.Data);

        //        foreach (var campaign in campaignViewModel)
        //        {
        //            var dateToday = DateTime.Now.Date;
        //            if (dateToday >= campaign.StartDate && dateToday <= campaign.EndDate)
        //            {
        //                campaign.Status = StatusEnum.Active;
        //            }
        //            else if (dateToday >= campaign.StartDate && dateToday >= campaign.EndDate)
        //            {
        //                campaign.Status = StatusEnum.Expired;
        //            }
        //            else if (dateToday < campaign.StartDate && dateToday < campaign.EndDate)
        //            {
        //                campaign.Status = StatusEnum.Inactive;
        //            }
        //        }

        //        var viewModel = new CampaignMappingsCreateModel
        //        {
        //            StoreId = storeId,
        //            Campaigns = campaignViewModel.Where(m => m.Status == StatusEnum.Active || m.Status == StatusEnum.Inactive).ToList()
        //        };

        //        return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignCampaigns", viewModel) });
        //    }
        //    return null;
        //}

        //[HttpPost]
        //public async Task<IActionResult> OnPostAssignCampaigns(int storeId, string selectedIds)
        //{
        //    List<int> campaignIds = JsonConvert.DeserializeObject<List<int>>(selectedIds);
        //    bool succeed = true;
        //    string failedIds = string.Empty;
        //    try
        //    {
        //        foreach (var id in campaignIds)
        //        {
        //            var existingMapping = _campaignMappingsRepository.CampaignMappings.Where(m => m.StoreId == storeId && m.CampaignId == id && m.IsDeleted == true).FirstOrDefault();
        //            Result<int> result;
        //            if (existingMapping == null)
        //            {
        //                var createCampaignMappings = new CreateCampaignMappingsCommand
        //                {
        //                    CampaignId = id,
        //                    StoreId = storeId,
        //                };

        //                result = await _mediator.Send(createCampaignMappings);
        //            }
        //            else
        //            {
        //                existingMapping.IsDeleted = false;
        //                var updateCampaignMappingModel = _mapper.Map<UpdateCampaignMappingsCommand>(existingMapping);
        //                result = await _mediator.Send(updateCampaignMappingModel);
        //            }

        //            if (result.Failed)
        //            {
        //                failedIds += ", " + result.Message;
        //            };
        //        }

        //        if (!string.IsNullOrEmpty(failedIds))
        //        {
        //            failedIds = failedIds.Remove(0, 2);
        //            succeed = false;
        //        }

        //        if (succeed)
        //            _notify.Success($"Campaigns Assigned to Store With Id {storeId}.");
        //        else
        //            _notify.Error($"Failed to Assign Campaigns with Id ({failedIds}) to Store With Id {storeId}", 5);

        //        var response = await _mediator.Send(new GetAllStoresCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<StoreViewModel>>(response.Data);
        //            var storesList = new List<StoreViewModel>();

        //            foreach (var model in viewModel)
        //            {
        //                var storesViewModel = new StoreViewModel()
        //                {
        //                    Id = model.Id,
        //                    State = model.State,
        //                    Name = model.Name,
        //                    CreatedOn = model.CreatedOn,
        //                    IsActive = model.IsActive,
        //                };
        //                storesList.Add(storesViewModel);
        //            }

        //            var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", storesList);
        //            return new JsonResult(new { isValid = true, html });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _notify.Error("Failed To Assign Campaigns.");
        //        return null;
        //    }
        //    return default;
        //}
        #endregion
    }
}

