using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class SiteSetting
    {
        public int Id { get; set; }

        public int? GroupId { get; set; }
        public SettingGroup? Group { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }

        public string? Value { get; set; }

        public string? DefaultValue { get; set; }

        [Required]
        public string DataType { get; set; } // "string", "number", "boolean", "color", "image", "text", "html", "json"

        public string? Description { get; set; }

        public bool IsPublic { get; set; } = true;

        public bool Editable { get; set; } = true;

        public int Order { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<SettingHistory> History { get; set; } = new List<SettingHistory>();
    }
}
