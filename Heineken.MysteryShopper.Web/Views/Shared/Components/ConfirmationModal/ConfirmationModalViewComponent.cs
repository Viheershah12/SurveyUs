using Microsoft.AspNetCore.Mvc;

namespace SurveyUs.Web.Views.Shared.Components.ConfirmationModal
{
    public class ConfirmationModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
