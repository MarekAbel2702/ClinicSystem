using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.Models
{
    public class Doctor
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

        [Required(ErrorMessage = "Numer PWZ jest wymagany")]
        [StringLength(20, ErrorMessage = "Numer PWZ może mieć maksymalnie 20 znaków")]
        [Display(Name = "Numer PWZ")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Niepoprawny numer telefonu")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [StringLength(1000, ErrorMessage = "Opis może mieć maksymalnie 1000 znaków")]
        [Display(Name = "Opis lekarza")]
        public string? Description { get; set; }

        [Display(Name = "Numer gabinetu")]
        public string? OfficeNumber { get; set; }

        [Display(Name = "Aktywny")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Specjalizacja")]
        public int SpecializationId { get; set; }

        [Display(Name = "Specjalizacja")]
        public Specialization? Specialization { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
