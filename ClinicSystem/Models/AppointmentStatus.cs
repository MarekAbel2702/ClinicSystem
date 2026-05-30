using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.Models
{
    public enum AppointmentStatus
    {
        [Display(Name = "Zaplanowana")]
        Scheduled = 1,

        [Display(Name = "Zrealizowana")]
        Completed = 2,

        [Display(Name = "Odwołana")]
        Cancelled = 3,

        [Display(Name = "Pacjent nieobecny")]
        NoShow = 4
    }
}
