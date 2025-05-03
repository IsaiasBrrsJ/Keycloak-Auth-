

using System.ComponentModel.DataAnnotations;

namespace AuthWithKeyCloak.KeyCloak
{
    public class LoginOption
    {
        [Required]
        public string clientId { get; set; } = string.Empty;
        [Required]
        public string clientSecret { get; set; } = string.Empty;
        [Required]
        public string grantType { get; set; } = string.Empty;
        
    }
}
