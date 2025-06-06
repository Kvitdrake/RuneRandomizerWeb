﻿namespace webb_tst_site3.Models
{
    public class SettingHistory
    {
        public int Id { get; set; }
        public int? ChangedByUserId { get; set; }
        public User ChangedByUser { get; set; }

        public int SettingId { get; set; }
        public SiteSetting Setting { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
