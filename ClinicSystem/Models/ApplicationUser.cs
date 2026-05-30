using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ClinicSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        [Display(Name = "Imię")]
        public string? FirstName { get; set; }

        [StringLength(80)]
        [Display(Name = "Nazwisko")]
        public string? LastName { get; set; }

        [Display(Name = "Data utworzenia konta")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Patient? Patient { get; set; }
    }
}