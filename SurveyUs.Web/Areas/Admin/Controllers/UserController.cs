using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyUs.Application.Constants;
using SurveyUs.Application.Enums;
using SurveyUs.Application.Features.UserExtension.Commands.Create;
using SurveyUs.Application.Features.UserExtension.Commands.Delete;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Areas.Users.Models;
using SurveyUs.Web.Extensions;

namespace SurveyUs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : BaseController<UserController>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHostingEnvironment Environment;
        private readonly IUserExtensionRepository _userExtensionRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IConfiguration _config;

        private const string _userDataTable = "userTable";

        public UserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IHostingEnvironment _hostingEnvironment,
            IUserExtensionRepository userExtensionRepository,
            IStoreRepository storeRepository,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            Environment = _hostingEnvironment;
            _userExtensionRepository = userExtensionRepository;
            _storeRepository = storeRepository;
            _config = config;
        }

        [Authorize(Policy = Permissions.Users.View)]
        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> LoadAll()
        //{
        //    await GetUsersListing(new UserDataTable { length = 10, start = 0 });

        //    var usersList = await GetUsersList();

        //    return PartialView("_ViewAll", usersList);
        //}

        [HttpPost]
        public async Task<JsonResult> GetUsersListing(DataTableExtensions request)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var con = _config.GetConnectionString("ApplicationConnection");
            var pageNumber = Convert.ToInt32(Math.Ceiling((request.start + 1.0) / request.length));

            List<UserViewModel> userList = new List<UserViewModel>();

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
                    SELECT u.*, r.Name AS ""Role""
                    FROM [Identity].Users u
                    LEFT JOIN [Identity].UserRoles ur on ur.UserId = u.Id
                    LEFT JOIN [Identity].Roles r on r.Id = ur.RoleId
                    WHERE u.Id <> @currentUserId AND r.Name = 'Mystery Drinker'";

            if (!string.IsNullOrEmpty(searchValue))
            {
                paramName.Add("@searchTerm");
                paramValue.Add(searchValue);

                rawQuery += @"
                    AND LOWER(u.FirstName) LIKE '%' + @searchTerm + '%' 
                    OR LOWER(u.LastName) LIKE '%' + @searchTerm + '%'
                    OR LOWER(u.Email) LIKE '%' + @searchTerm + '%'
                    OR LOWER(r.Name) LIKE '%' + @searchTerm + '%'
                    OR CONCAT(u.FirstName, ' ', u.LastName) LIKE '%' + @searchTerm + '%' ";
            }

            rawQuery += "ORDER BY ";

            if (sortColumnName == "firstname")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"u.FirstName ";
                }  
                else
                    rawQuery += @"u.FirstName DESC ";
            }
            else if (sortColumnName == "email")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"u.Email ";
                }
                else
                    rawQuery += @"u.Email DESC ";
            }
            else if (sortColumnName == "isactive")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"u.IsActive ";
                }
                else
                    rawQuery += @"u.IsActive DESC ";
            }
            else if (sortColumnName == "role")
            {
                if (sortColumnDirection == "asc")
                {
                    rawQuery += @"r.Name ";
                }
                else
                    rawQuery += @"r.Name DESC ";
            }
            else
            {
                rawQuery += "u.CreatedOn DESC ";
            }
            #endregion

            var selectQuery = new StringBuilder();
            selectQuery.Append(rawQuery);

            paramName.Add("@currentUserId");
            paramValue.Add(currentUser.Id);

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

                        var res = new UserViewModel()
                        {
                            Id = dr["Id"] is DBNull ? null : Convert.ToString(dr["Id"]),
                            FirstName = dr["FirstName"] is DBNull ? null : Convert.ToString(dr["FirstName"]),
                            LastName = dr["LastName"] is DBNull ? null : Convert.ToString(dr["LastName"]),
                            Email = dr["Email"] is DBNull ? null : Convert.ToString(dr["Email"]),
                            Role = dr["Role"] is DBNull ? null : Convert.ToString(dr["Role"]),
                            IsActive = dr["IsActive"] is DBNull ? false : Convert.ToBoolean(dr["IsActive"]),
                            CreatedOn = dr["CreatedOn"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(dr["CreatedOn"]),
                        };
                        userList.Add(res);
                    }
                }

                //var sortColumn = request.order[0]?.column;
                //string sortColumnDirection = request.order[0]?.dir;
                //var searchValue = request.search.value?.ToLower().Trim();

                //if (sortColumn != null && !string.IsNullOrEmpty(sortColumnDirection))
                //{
                //    var sortColumnName = request.columns[(int)sortColumn].name;
                //    userList = userList.AsQueryable().OrderBy(sortColumnName + " " + sortColumnDirection).ToList();
                //}

                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    userList = userList.AsQueryable().Where(item => item.FirstName.ToLower().Contains(searchValue) 
                //                                            || item.LastName.ToLower().Contains(searchValue)
                //                                            || item.Email.ToLower().Contains(searchValue)
                //                                            || item.Role.ToLower().Contains(searchValue)
                //                                            || (item.FirstName + " " + item.LastName).ToLower().Contains(searchValue)).ToList();
                //}

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = userList,
                });
            }
        }

        //public async Task<List<UserViewModel>> GetUsersList()
        //{
        //    var usersList = new List<UserViewModel>();
        //    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        //    var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();

        //    try
        //    {
        //        foreach (var user in allUsersExceptCurrentUser)
        //        {
        //            var userRole = await _userManager.GetRolesAsync(user);
        //            string roles = "";
        //            var IsDeleted = false;

        //            if (userRole != null)
        //            {
        //                for (int i = 0; i < userRole.Count; i++)
        //                {
        //                    if (i == userRole.Count - 1)
        //                        roles += userRole[i];
        //                    else
        //                        roles += userRole[i] + ", ";
        //                }
        //            }

        //            if (userRole.Contains("Bartender"))
        //            {
        //                var userExtension = await _userExtensionRepository.GetByUserIdAsync(user.Id);
        //                if (userExtension != null)
        //                    IsDeleted = userExtension.IsDeleted;
        //            }

        //            if (!IsDeleted && !userRole.Contains("SuperAdmin"))
        //            {
        //                try
        //                {
        //                    if (user.LastName == null)
        //                        user.LastName = "";

        //                    var model = new UserViewModel
        //                    {
        //                        Id = user.Id,
        //                        FirstName = user.FirstName.ToUpper(),
        //                        LastName = user.LastName.ToUpper(),
        //                        Email = user.Email,
        //                        IsActive = user.IsActive,
        //                        Role = roles
        //                    };
        //                    usersList.Add(model);
        //                }
        //                catch (Exception e)
        //                {
        //                    var error = e.Message;
        //                    Console.WriteLine(error);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        var error = e.Message;
        //        Console.WriteLine(error);
        //    }

        //    return usersList;
        //}

        public async Task<IActionResult> OnGetCreate()
        {
            var model = new UserViewModel
            {
                DataTableId = _userDataTable
            };

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", model) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreate(UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {   
                try
                {
                    var user = new ApplicationUser
                    {
                        UserName = userModel.FirstName,
                        Email = userModel.Email,
                        FirstName = userModel.FirstName.ToUpper(),
                        LastName = userModel.LastName.ToUpper(),
                        EmailConfirmed = true,
                        CreatedOn = DateTime.Now
                    };
                    var createUserResult = await _userManager.CreateAsync(user, userModel.Password);

                    if (createUserResult.Succeeded)
                    {
                        var userExtensionViewModel = new UserExtensionViewModel()
                        {
                            UserId = user.Id,
                            Email = user.Email,
                            Name = user.FirstName + " " + user.LastName
                        };
                        var createUserExtensionCommand = _mapper.Map<CreateUserExtensionCommand>(userExtensionViewModel);
                        var createUserExtensionResult = await _mediator.Send(createUserExtensionCommand);
                        await _userManager.AddToRoleAsync(user, userModel.Role);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        _notify.Success($"Account for {user.Email} created.");
                        return new JsonResult(new { isValid = true, dataTableId = userModel.DataTableId });
                    }

                    foreach (var error in createUserResult.Errors)
                    {
                        _notify.Error(error.Description);
                    }
                }               
                catch (Exception ex)
                {
                    _notify.Error(ex.InnerException?.Message);
                    return new JsonResult(new { isValid = false, dataTableId = userModel.DataTableId });
                }
            }
            return new JsonResult(new { isValid = false, dataTableId = userModel.DataTableId });
        }

        public async Task<IActionResult> OnGetResetPassword(string userId)
        {
            var model = new UserViewModel();
            model.Id = userId;

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_ResetPassword", model) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostResetPassword(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var x = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (x.Succeeded)
            {
                _notify.Success($"Password for account {user.Email} has been changed.");
                return new JsonResult(new { isValid = true, dataTableId = model.DataTableId });
            }

            return new JsonResult(new { isValid = false, dataTableId = model.DataTableId });           
        }

        public async Task<IActionResult> OnGetEditUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var model = new UserViewModel();

            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.DataTableId = _userDataTable;

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Edit", model) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostEditUser(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var x = await _userManager.UpdateAsync(user);

            if (x.Succeeded)
            {
                _notify.Success($"Profile for account {user.Email} has been updated.");
                return new JsonResult(new { dataTableId = model.DataTableId, isValid = true });
            }

            _notify.Error($"Failed to update profile for account {user.Email}");
            return new JsonResult(new { dataTableId = model.DataTableId, isValid = false });
        }

        public async Task<IActionResult> AddMysteryShopper()
        {
            var mysteryShopperViewModel = new UserViewModel()
            {
                IsMysteryShopper = true,
                Role = "Mystery Drinker",
                DataTableId = _userDataTable
            };

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", mysteryShopperViewModel) });
        }

        //[HttpPost]
        //public async Task<IActionResult> OnPostAddParticipant(ParticipantViewModel participantModel)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var participant = participantModel;
        //            participant.Role = "Mystery Shopper";
        //            MailAddress address = new MailAddress(participantModel.Email);
        //            string userName = address.User;
        //            var user = new ApplicationUser
        //            {
        //                UserName = userName,
        //                Email = participantModel.Email,
        //                FirstName = participantModel.FirstName.ToUpper(),
        //                LastName = participantModel.LastName.ToUpper(),
        //                EmailConfirmed = true,
        //            };
        //            var result = await _userManager.CreateAsync(user, participantModel.Password);
        //            if (result.Succeeded)
        //            {
        //                await _userManager.AddToRoleAsync(user, participant.Role);
        //                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        //                var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
        //                var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
        //                var htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);
        //                _notify.Success($"Account for {user.Email} created.");
        //                return new JsonResult(new { isValid = true, html = htmlData });
        //            }
        //            foreach (var error in result.Errors)
        //            {
        //                _notify.Error(error.Description);
        //            }
        //            var html = await _viewRenderer.RenderViewToStringAsync("_AddParticipant", participantModel);
        //            return new JsonResult(new { isValid = false, html });
        //        }
        //        return default;
        //    }
        //    catch (Exception e)
        //    {
        //        var error = e.Message;
        //        Console.WriteLine(error);
        //    }

        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> OnActivateUser (string userId)
        {
            if(User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var user = await _userManager.FindByIdAsync(userId);
                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _notify.Success($"User '{user.FirstName + ' ' + user.LastName}' Activated.");
                    return new JsonResult(new { isValid = true, dataTableId = _userDataTable });
                }
                _notify.Success($"Failed To Activate User '{user.FirstName + ' ' + user.LastName}'.");
            }

            return new JsonResult(new { isValid = false });
        }

        public async Task<IActionResult> OnDeactivateUser(string userId)
        {
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var user = await _userManager.FindByIdAsync(userId);
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _notify.Success($"User '{user.FirstName + ' ' + user.LastName}' Deactivated.");
                    return new JsonResult(new { isValid = true, dataTableId = _userDataTable });
                }
                _notify.Success($"Failed To Deactivate User '{user.FirstName + ' ' + user.LastName}'.");
            }

            return new JsonResult(new { isValid = false });
        }

        public async Task<IActionResult> BulkUpload()
        {
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_BulkUpload", new BartenderViewModel()) });
        }

        [HttpPost]
        public async Task<IActionResult> SaveBartenderExcel(IFormFile File, BartenderViewModel bartenderModel)
        {
            try
            {
                var usersList = new List<UserViewModel>();
                var currentUser = new ApplicationUser();
                var allUsersExceptCurrentUser = new List<ApplicationUser>();
                var html = "";

                string path = Path.Combine(Environment.WebRootPath, "Uploads");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                List<string> uploadedFiles = new List<string>();

                if (File != null)
                {
                    string fileName = Path.GetFileName(File.FileName);
                    var filepath = Path.Combine(path, fileName);
                    var extension = Path.GetExtension(filepath).ToUpper();

                    if (extension == ".XLS" || extension == ".XLSX")
                    {
                        using (FileStream stream = new FileStream(filepath, FileMode.Create))
                        {
                            File.CopyTo(stream);
                            uploadedFiles.Add(fileName);
                        }

                        List<BartenderViewModel> bartenderList = new List<BartenderViewModel>();

                        //read uploaded excel files
                        using (var stream = System.IO.File.Open(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                //configuration to read first row as column name
                                var conf = new ExcelDataSetConfiguration
                                {
                                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                                    {
                                        UseHeaderRow = true
                                    }
                                };

                                var dataSet = reader.AsDataSet(conf);
                                var dataTable = dataSet.Tables[0];

                                if (dataTable.Columns.Count == 15)
                                {
                                    for (int i = 0; i < dataTable.Rows.Count; i++)
                                    {

                                        //Getting the Day and Month of the designation
                                        var substrings = dataTable.Rows[i][13].ToString().TrimEnd().Split(' ');
                                        var designation = substrings[0] + " " + substrings[1];

                                        bartenderList.Add(new BartenderViewModel
                                        {
                                            Name = dataTable.Rows[i][5].ToString().ToUpper(),
                                            Email = dataTable.Rows[i][3].ToString(),
                                            Ic = Regex.Replace(dataTable.Rows[i][6].ToString(), @"[^0-9a-zA-Z]+", ""),
                                            Gender = dataTable.Rows[i][8].ToString(),
                                            Telephone = dataTable.Rows[i][7].ToString(),
                                            Outlet = dataTable.Rows[i][9].ToString(),
                                            OutletAddress = dataTable.Rows[i][10].ToString(),
                                            OutletLocation = dataTable.Rows[i][11].ToString().Trim(),
                                            JoiningAs = dataTable.Rows[i][12].ToString(),
                                            UniformSize = dataTable.Rows[i][14].ToString(),
                                            Designation = designation
                                        });
                                    }
                                }
                                else
                                {
                                    _notify.Error("Excel Data Is Invalid");

                                    usersList = new List<UserViewModel>();
                                    currentUser = await _userManager.GetUserAsync(HttpContext.User);
                                    allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
                                    foreach (var user in allUsersExceptCurrentUser)
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

                                        if (user.LastName == null)
                                            user.LastName = "";

                                        var model = new UserViewModel()
                                        {
                                            Id = user.Id,
                                            FirstName = user.FirstName.ToUpper(),
                                            LastName = user.LastName.ToUpper(),
                                            Email = user.Email,
                                            IsActive = user.IsActive,
                                            Role = roles
                                        };
                                        usersList.Add(model);
                                    }

                                    html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", usersList);
                                    return new JsonResult(new { isValid = true, html });
                                }
                            }
                        }

                        //delete the file after data is taken
                        System.IO.File.Delete(Path.Combine(path, fileName));

                        //saving data into database
                        if (bartenderList.Count != 0)
                        {
                            var existingBartenders = new List<UserExtension>();

                            if (bartenderModel.Designation == "All")
                            {
                                //get all existing bartenders in UserExtension table
                                existingBartenders = await _userExtensionRepository.GetListAsync();
                            }
                            else
                            {
                                var designation = 0;

                                //Getting the day and month from the designation
                                var substrings = bartenderModel.Designation.Split(' ');
                                bartenderModel.Designation = substrings[0] + " " + substrings[1];

                                designation = bartenderModel.Designation switch
                                {
                                    "15th May" => (int)StateEnum.Sabah,
                                    "17th May" => (int)StateEnum.Sarawak,
                                    "12th June" => (int)StateEnum.Selangor,
                                    "26th June" => (int)StateEnum.Johor,
                                    "3rd July" => (int)StateEnum.PulauPinang,
                                    "5th July" => (int)StateEnum.Perak,
                                    _ => (int)StateEnum.WPKualaLumpur,
                                };

                                var store = await _storeRepository.GetByStateAsync(designation);

                                //filter bartender if designation being specified
                                bartenderList = bartenderList.Where(x => x.Designation == bartenderModel.Designation).ToList();

                                //get all existing bartenders with specified designation in UserExtension table
                                existingBartenders = await _userExtensionRepository.GetByStoreAsync(store.Id);
                            }

                            //delete all existing bartenders in Users & UserExtension table and all their saved answers
                            foreach (var existingBartender in existingBartenders)
                            {
                                var user = await _userManager.FindByIdAsync(existingBartender.UserId);
                                if (user != null)
                                    await _userManager.DeleteAsync(user);
                                await _mediator.Send(new DeleteUserExtensionCommand { Id = existingBartender.Id });
                            }

                            //adding bartenders into database
                            foreach (var bartender in bartenderList)
                            {
                                var registeredUser = await _userManager.FindByNameAsync(bartender.Ic.ToUpper());

                                var designation = 0;

                                //Getting the day and month from the designation
                                var substrings = bartender.Designation.Split(' ');
                                bartenderModel.Designation = substrings[0] + " " + substrings[1];

                                designation = bartenderModel.Designation switch
                                {
                                    "15th May" => (int)StateEnum.Sabah,
                                    "17th May" => (int)StateEnum.Sarawak,
                                    "12th June" => (int)StateEnum.Selangor,
                                    "26th June" => (int)StateEnum.Johor,
                                    "3rd July" => (int)StateEnum.PulauPinang,
                                    "5th July" => (int)StateEnum.Perak,
                                    _ => (int)StateEnum.WPKualaLumpur,
                                };

                                //user exists in Users table
                                if (registeredUser != null)
                                {
                                    //insert bartender to UserExtension table
                                    var registeredBartender = await _userExtensionRepository.GetByUserIdAsync(registeredUser.Id);

                                    //bartender is not yet registered
                                    if (registeredBartender == null)
                                    {
                                        var store = await _storeRepository.GetByStateAsync(designation);

                                        if (store != null)
                                            bartender.Store = store.Id;

                                        bartender.UserId = registeredUser.Id;

                                        var createUserExtensionCommand = _mapper.Map<CreateUserExtensionCommand>(bartender);
                                        await _mediator.Send(createUserExtensionCommand);
                                    }
                                }
                                else
                                {
                                    //create users
                                    MailAddress address = new MailAddress(bartender.Ic + "@gmail.com");
                                    string userName = address.User.ToUpper();

                                    var user = new ApplicationUser
                                    {
                                        UserName = userName,
                                        Email = userName + "@gmail.com",
                                        FirstName = bartender.Name.ToUpper(),
                                        LastName = "",
                                        EmailConfirmed = true,
                                    };

                                    var result = await _userManager.CreateAsync(user, userName + "Abcd1234");

                                    if (result.Succeeded)
                                    {
                                        await _userManager.AddToRoleAsync(user, Roles.Bartender.ToString());

                                        var store = await _storeRepository.GetByStateAsync(designation);

                                        registeredUser = await _userManager.FindByNameAsync(userName);

                                        if (store != null)
                                            bartender.Store = store.Id;

                                        bartender.UserId = registeredUser.Id;

                                        var createUserExtensionCommand = _mapper.Map<CreateUserExtensionCommand>(bartender);
                                        await _mediator.Send(createUserExtensionCommand);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _notify.Error("Wrong File Extension");
                    }
                }
                else
                {
                    _notify.Error("No File Uploaded");
                }

                _notify.Success("Bulk upload success.");
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                var error = e.Message;
                Console.WriteLine(error);
                _notify.Error(error);
            }

            return RedirectToAction("Index");
        }
    }
}