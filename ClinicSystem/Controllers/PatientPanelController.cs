using ClinicSystem.Data;
using ClinicSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicSystem.Models;

namespace ClinicSystem.Controllers
{
    [Authorize(Roles = "Pacjent")]
    public class PatientPanelController : Controller
    {
        private readonly ClinicDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientPanelController(
            ClinicDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var patient = await _context.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

            if (patient == null)
            {
                return View("NoPatientProfile");
            }

            var now = DateTime.Now;

            var viewModel = new PatientPanelViewModel
            {
                Patient = patient,

                UpcomingAppointments = patient.Appointments
                    .Where(a => a.AppointmentDate >= now && a.Status == AppointmentStatus.Scheduled)
                    .OrderBy(a => a.AppointmentDate)
                    .ToList(),

                PastAppointments = patient.Appointments
                    .Where(a => a.AppointmentDate < now || a.Status != AppointmentStatus.Scheduled)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToList(),

                AllAppointmentsCount = patient.Appointments.Count,

                UpcomingAppointmentsCount = patient.Appointments.Count(a =>
                    a.AppointmentDate >= now && a.Status == AppointmentStatus.Scheduled),

                CompletedAppointmentsCount = patient.Appointments.Count(a =>
                    a.Status == AppointmentStatus.Completed)
            };

            return View(viewModel);
        }
    }
}