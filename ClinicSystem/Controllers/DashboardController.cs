using ClinicSystem.Data;
using ClinicSystem.Models;
using ClinicSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
    public class DashboardController : Controller
    {
        private readonly ClinicDbContext _context;

        public DashboardController(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var viewModel = new DashboardViewModel
            {
                PatientsCount = await _context.Patients.CountAsync(),
                DoctorsCount = await _context.Doctors.CountAsync(),
                SpecializationsCount = await _context.Specializations.CountAsync(),
                AppointmentsCount = await _context.Appointments.CountAsync(),

                TodayAppointmentsCount = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow),

                ScheduledAppointmentsCount = await _context.Appointments
                    .CountAsync(a => a.Status == AppointmentStatus.Scheduled),

                CompletedAppointmentsCount = await _context.Appointments
                    .CountAsync(a => a.Status == AppointmentStatus.Completed),

                CancelledAppointmentsCount = await _context.Appointments
                    .CountAsync(a => a.Status == AppointmentStatus.Cancelled),

                UpcomingAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                    .Where(a => a.AppointmentDate >= DateTime.Now && a.Status == AppointmentStatus.Scheduled)
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5)
                    .ToListAsync(),

                RecentAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}