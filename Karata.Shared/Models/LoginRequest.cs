using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Karata.Shared.Models
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
