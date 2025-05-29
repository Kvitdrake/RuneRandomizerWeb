using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class SettingGroup
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int Order { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SiteSetting> Settings { get; set; } = new List<SiteSetting>();
    }
}
