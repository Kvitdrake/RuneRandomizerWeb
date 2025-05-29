using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace webb_tst_site3.Pages.Admin.Settings
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<SettingGroup> SettingGroups { get; set; }
        public List<SiteSetting> Settings { get; set; }
        public int ActiveGroupId { get; set; }

        public async Task OnGetAsync(int? groupId)
        {
            SettingGroups = await _context.SettingGroups
                .OrderBy(g => g.Order)
                .ToListAsync();

            Settings = await _context.SiteSettings
                .Include(s => s.Group)
                .OrderBy(s => s.Order)
                .ToListAsync();

            ActiveGroupId = groupId ?? SettingGroups.FirstOrDefault()?.Id ?? 0;
        }

        public async Task<IActionResult> OnPostUpdateGroupAsync(int groupId, Dictionary<int, SiteSetting> settings)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var setting in settings)
            {
                var dbSetting = await _context.SiteSettings.FindAsync(setting.Key);
                if (dbSetting != null && dbSetting.Editable)
                {
                    dbSetting.Value = setting.Value.Value;
                    dbSetting.UpdatedAt = DateTime.UtcNow;

                    // Запись в историю изменений
                    _context.SettingHistory.Add(new SettingHistory
                    {
                        SettingId = dbSetting.Id,
                        OldValue = dbSetting.Value,
                        NewValue = setting.Value.Value,
                        ChangedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { groupId });
        }
    }
}