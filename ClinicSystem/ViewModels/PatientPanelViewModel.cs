using ClinicSystem.Models;

namespace ClinicSystem.ViewModels
{
    public class PatientPanelViewModel
    {
        public Patient? Patient { get; set; }

        public List<Appointment> UpcomingAppointments { get; set; } = new();

        public List<Appointment> PastAppointments { get; set; } = new();

        public int AllAppointmentsCount { get; set; }

        public int UpcomingAppointmentsCount { get; set; }

        public int CompletedAppointmentsCount { get; set; }
    }
}