using System.ComponentModel.DataAnnotations;

namespace StudentGradingMVC.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
