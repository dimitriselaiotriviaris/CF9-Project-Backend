using System.ComponentModel.DataAnnotations;

namespace CF9Project.DTO
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public int? RoleId { get; set; }
    }
}
