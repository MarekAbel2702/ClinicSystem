using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }
}