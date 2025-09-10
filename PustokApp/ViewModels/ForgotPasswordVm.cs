using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class ForgotPasswordVm
    {
        [Required(ErrorMessage = "Email ünvanı tələb olunur")]
        [EmailAddress(ErrorMessage = "Email ünvanı düzgün formatda deyil")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}