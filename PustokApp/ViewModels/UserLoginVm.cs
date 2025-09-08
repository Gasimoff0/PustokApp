using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class UserLoginVm
    {
        [Required]
        public string UserNameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
