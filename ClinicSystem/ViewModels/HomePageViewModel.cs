using ClinicSystem.Models;

namespace ClinicSystem.ViewModels
{
    public class HomePageViewModel
    {
        public int PatientsCount { get; set; }
        public int DoctorsCount { get; set; }
        public int SpecializationsCount { get; set; }
        public int AppointmentsCount { get; set; }

        public List<Doctor> FeaturedDoctors { get; set; } = new();
        public List<Specialization> Specializations { get; set; } = new();
    }
}