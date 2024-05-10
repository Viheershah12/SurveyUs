using System.Threading.Tasks;

namespace SurveyUs.Web.Abstractions
{
    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);

        Task<string> RenderToStringAsync<T>(string pageName, T model);
    }
}