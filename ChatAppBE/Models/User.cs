using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChatAppBE.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string PasswordHash { get; set; } // Mật khẩu đã hash
    }
}
