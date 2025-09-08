using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class UserUpdateProfileVm
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
