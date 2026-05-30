using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Data i godzina wizyty są wymagane")]
        [Display(Name = "Data i godzina wizyty")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Powód wizyty")]
        [StringLength(300, ErrorMessage = "Powód wizyty może miećmaksymalnie 300 znaków")]
        [Display(Name = "Powód wizyty")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Status wizyty")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [StringLength(1000, ErrorMessage = "Notatka może mieć maksymalnie 1000 znaków")]
        [Display(Name = "Notatka lekarza")]
        public string? DoctorNote { get; set; }

        [StringLength(1000, ErrorMessage = "Diagnoza może mieć maksymalnie 1000 znaków")]
        [Display(Name = "Diagnoza")]
        public string? Diagnosis { get; set; }

        [StringLength(1000, ErrorMessage = "Zalecenia mogą mieć maksymalnie 1000 znaków")]
        [Display(Name = "Zalecenia")]
        public string? Recomendations { get; set; }

        [Display(Name = "Data utworzenia")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Pacjent jest wymagany")]
        [Display(Name = "Pacjent")]
        public int PatientId { get; set; }

        [Display(Name = "Pacjent")]
        public Patient? Patient { get; set; }

        [Required(ErrorMessage = "Lekarz jest wymagany")]
        [Display(Name = "Lekarz")]
        public int DoctorId { get; set; }

        [Display(Name = "Lekarz")]
        public Doctor? Doctor { get; set; }
    }
}
