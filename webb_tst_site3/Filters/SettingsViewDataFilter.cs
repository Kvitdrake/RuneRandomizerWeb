using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Services;

namespace webb_tst_site3.Filters
{
    public class SettingsViewDataFilter : IAsyncActionFilter
    {
        private readonly SettingsService _settingsService;

        public SettingsViewDataFilter(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var settings = await _settingsService.GetPublicSettingsAsync();
            var controller = context.Controller as PageModel;

            if (controller != null)
            {
                foreach (var setting in settings)
                {
                    controller.ViewData[setting.Key] = setting.Value;
                }

                // Убедимся, что SiteName всегда установлен
                if (!controller.ViewData.ContainsKey("SiteName"))
                {
                    controller.ViewData["SiteName"] = await _settingsService.GetSiteNameAsync();
                }
            }

            await next();
        }
    }
}