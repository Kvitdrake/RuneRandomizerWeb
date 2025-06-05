using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // "Admin" или "User"
    }
}