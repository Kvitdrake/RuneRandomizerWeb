using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Services
{
    public class SettingsService
    {
        private readonly AppDbContext _context;
        private Dictionary<string, string> _settingsCache;
        private DateTime _lastCacheUpdate;

        public SettingsService(AppDbContext context)
        {
            _context = context;
            _settingsCache = new Dictionary<string, string>();
            _lastCacheUpdate = DateTime.MinValue;
        }
        public async Task<string> GetSiteNameAsync()
        {
            return await GetSettingAsync("SiteName", "Soltias Site");
        }
        public async Task<string> GetSettingAsync(string name, string defaultValue = "")
        {
            await EnsureCacheIsUpToDate();

            if (_settingsCache.TryGetValue(name, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public async Task<Dictionary<string, string>> GetPublicSettingsAsync()
        {
            await EnsureCacheIsUpToDate();
            return _settingsCache.Where(s => s.Value != null).ToDictionary(s => s.Key, s => s.Value);
        }

        private async Task EnsureCacheIsUpToDate()
        {
            if ((DateTime.UtcNow - _lastCacheUpdate).TotalMinutes > 5)
            {
                var settings = await _context.SiteSettings
                    .Where(s => s.IsPublic)
                    .ToListAsync();

                _settingsCache = settings.ToDictionary(s => s.Name, s => s.Value);
                _lastCacheUpdate = DateTime.UtcNow;
            }
        }
        public async Task UpdateSettingAsync(string name, string value)
        {
            var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.Name == name);
            if (setting != null && setting.Editable)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _lastCacheUpdate = DateTime.MinValue; // Сбрасываем кэш
            }
        }
    }
}