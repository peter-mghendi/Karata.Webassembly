using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Karata.Shared.Models
{
    public class SignupRequest
    {
        [Required, StringLength(24, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 16 characters long.")]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required, EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
