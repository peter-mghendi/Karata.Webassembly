using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Karata.Shared.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
