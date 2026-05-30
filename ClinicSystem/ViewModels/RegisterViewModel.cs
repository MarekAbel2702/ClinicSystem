using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Imię może mieć maksymalnie 50 znaków")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(80, ErrorMessage = "Nazwisko może mieć maksymalnie 80 znaków")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "PESEL jest wymagany")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "PESEL musi mieć dokładnie 11 cyfr")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "PESEL może zawierać tylko cyfry")]
        [Display(Name = "PESEL")]
        public string Pesel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        [DataType(DataType.Date)]
        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18);

        [Phone(ErrorMessage = "Niepoprawny numer telefonu")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [StringLength(150, ErrorMessage = "Adres może mieć maksymalnie 150 znaków")]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć minimum 6 znaków")]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są takie same")]
        [Display(Name = "Powtórz hasło")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}