using System.ComponentModel.DataAnnotations;

namespace WebApp_Anti.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Manager"; // Manager, VP, Admin
    }
}
