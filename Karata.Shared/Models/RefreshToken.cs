using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Karata.Shared.Models
{
    [Table("refresh_tokens")]
    public class RefreshToken
    {
        [Key, Column("token_string")]
        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        [Column("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }    // can be used for usage tracking
                                             // can optionally include other metadata, such as user agent, ip address, device name, and so on

        [Column("expire_at")]
        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }
}
