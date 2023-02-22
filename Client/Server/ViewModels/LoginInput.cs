using System.ComponentModel.DataAnnotations;

namespace Winux.Server.ViewModels
{
    public class LoginInput
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}