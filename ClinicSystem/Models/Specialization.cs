using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.Models
{
    public class Specialization
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa specjalizacji jest wymagana")]
        [StringLength(100, ErrorMessage = "Nazwa może mieć maksymalnie 100 znaków")]
        [Display(Name = "Nazwa specjalizacji")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Opis może mieć maksymalnie 500 znaków")]
        [Display(Name = "Opis")]
        public string? Description { get; set; }

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
