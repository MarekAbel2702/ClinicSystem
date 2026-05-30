using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Imię może mieć maksymalnie 50 znaków")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(80, ErrorMessage = "Nazwisko może mieć maksymalnie 80 znaków")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "PESEL jest wymagany")]
        [StringLength(11, ErrorMessage = "PESEL musi miećdokładnie 11 cyfr")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "PESEL może zawierać tylko cyfry")]
        [Display(Name = "PESEL")]
        public string PESEL { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        [DataType(DataType.Date)]
        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; }

        [Phone(ErrorMessage = "Niepoprawny numer telefonu")]
        [Display(Name  = "Telefon")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [StringLength(150, ErrorMessage = "Adres może mieć maksymalnie 150 znaków")]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [Display(Name = "Data rejestracji")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Id użytkownika")]
        public string? ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
