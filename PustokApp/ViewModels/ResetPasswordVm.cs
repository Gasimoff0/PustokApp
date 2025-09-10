using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class ResetPasswordVm
    {
        [Required(ErrorMessage = "Email ünvan; tələb olunur")]
        [EmailAddress(ErrorMessage = "Email ünvan düzgün formatda deyil")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token tələb olunur")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Yeni Şifrə tələb olunur")]
        [MinLength(6, ErrorMessage = "Şifr?ə Ən azı 6 simvoldan ibarət olmalıdır")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifrə təkrarı tələb olunur")]
        [MinLength(6, ErrorMessage = "Şifrə Ən azı 6 simvoldan ibarət olmalıdır")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Şifrələr eyni olmalıdır")]
        public string ConfirmPassword { get; set; }
    }
}