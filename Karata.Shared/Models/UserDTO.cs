using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Karata.Shared.Models
{
    public class UserDTO
    {
        public long Id { get; set; }

        [Required, StringLength(24, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 16 characters long.")]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
