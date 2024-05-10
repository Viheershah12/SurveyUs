using Microsoft.AspNetCore.Mvc;

namespace SurveyUs.Web.Views.Shared.Components.Title
{
    public class TitleViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}