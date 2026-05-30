using ClinicSystem.Models;

namespace ClinicSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int PatientsCount { get; set; }
        public int DoctorsCount { get; set; }
        public int SpecializationsCount { get; set; }
        public int AppointmentsCount { get; set; }

        public int TodayAppointmentsCount { get; set; }
        public int ScheduledAppointmentsCount { get; set; }
        public int CompletedAppointmentsCount { get; set; }
        public int CancelledAppointmentsCount { get; set; }

        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public List<Appointment> RecentAppointments { get; set; } = new();
    }
}