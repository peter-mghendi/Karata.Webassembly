using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Karata.Shared.Models
{
    public class ImpersonationRequest
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
