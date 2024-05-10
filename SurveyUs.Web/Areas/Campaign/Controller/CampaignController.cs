using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyUs.Application.Features.Campaign.Queries.GetAllCached;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Enums;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Abstractions;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Campaign.Controller
{
    [Area("Campaign")]

    public class CampaignController : BaseController<CampaignController>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;
        private readonly IStoreMappingsRepository _storeMappingsRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CampaignController(ICampaignRepository campaignRepository,
            ICampaignMappingsRepository campaignMappingsRepository,
            IStoreMappingsRepository storeMappingsRepository,
            UserManager<ApplicationUser> userManager
        )
        {
            _campaignRepository = campaignRepository;
            _campaignMappingsRepository = campaignMappingsRepository;
            _storeMappingsRepository = storeMappingsRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                return RedirectToAction("Index", "user", new { area = "Admin" });

            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            if (User.IsInRole("Mystery Drinker"))
            {
                return RedirectToAction("LoadAllForMysteryShopper");
            }

            var campaignResponses = await _mediator.Send(new GetAllCampaignsCachedQuery());
            var viewModel = new List<CampaignSettingViewModel>();

            if (campaignResponses.Succeeded && campaignResponses.Data.Count > 0)
            {
                foreach (var campaign in campaignResponses.Data)
                {
                    var campaignViewModel = new CampaignSettingViewModel
                    {
                        Id = campaign.Id,
                        Name = campaign.Name,
                        StartDate = campaign.StartDate,
                        EndDate = campaign.EndDate,
                    };

                    var dateToday = DateTime.Now.Date;
                    if (dateToday >= campaign.StartDate && dateToday <= campaign.EndDate)
                    {
                        campaignViewModel.Status = StatusEnum.Active;
                    }
                    else if (dateToday >= campaign.StartDate && dateToday >= campaign.EndDate)
                    {
                        campaignViewModel.Status = StatusEnum.Expired;
                    }
                    else if (dateToday < campaign.StartDate && dateToday < campaign.EndDate)
                    {
                        campaignViewModel.Status = StatusEnum.Inactive;
                    }

                    viewModel.Add(campaignViewModel);
                }
            }

            return PartialView("_ViewAll", viewModel);
        }

        public async Task<IActionResult> LoadAllForMysteryShopper()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var storeIds = _storeMappingsRepository.StoreMappings.Where(x => x.UserId == userId).Select(x => x.StoreId).ToList();
            var campaignIds = _campaignMappingsRepository.CampaignMappings.Where(c => storeIds.Contains(c.StoreId)).Select(c => c.CampaignId).ToList();
            var campaigns = await _campaignRepository.Campaign.Where(c => campaignIds.Contains(c.Id)).ToListAsync();

            var viewModel = _mapper.Map<List<CampaignSettingViewModel>>(campaigns);

            foreach (var campaign in viewModel)
            {
                var dateToday = DateTime.Now.Date;
                if (dateToday >= campaign.StartDate && dateToday <= campaign.EndDate)
                {
                    campaign.Status = StatusEnum.Active;
                }
                else if (dateToday >= campaign.StartDate && dateToday >= campaign.EndDate)
                {
                    campaign.Status = StatusEnum.Expired;
                }
                else if (dateToday < campaign.StartDate && dateToday < campaign.EndDate)
                {
                    campaign.Status = StatusEnum.Inactive;
                }
            }

            viewModel = viewModel.Where(c => c.Status == StatusEnum.Active || c.Status == StatusEnum.Expired).ToList();

            return PartialView("_ViewAll", viewModel);
        }
    }
}
