using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Karata.Server.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [Column("username")]
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Column("email")]
        [Required, EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Column("password")]
        [Required]
        [JsonPropertyName("password")]
        public byte[] Password { get; set; }

        [Column("salt")]
        [Required]
        [JsonPropertyName("salt")]
        public byte[] Salt { get; set; }

        [Column("role")]
        [Required]
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
