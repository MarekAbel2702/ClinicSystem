using ClinicSystem.Data;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ClinicDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(
                ClinicDbContext context,
                UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
        public async Task<IActionResult> Index(
            string? searchString,
            int? doctorId,
            AppointmentStatus? status,
            DateTime? date)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentDoctor"] = doctorId;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentDate"] = date?.ToString("yyyy-MM-dd");

            await LoadSelectListsAsync(doctorId: doctorId);

            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                appointments = appointments.Where(a =>
                    a.Patient!.FirstName.Contains(searchString) ||
                    a.Patient.LastName.Contains(searchString) ||
                    a.Patient.PESEL.Contains(searchString) ||
                    a.Doctor!.FirstName.Contains(searchString) ||
                    a.Doctor.LastName.Contains(searchString) ||
                    a.Reason.Contains(searchString));
            }

            if (doctorId.HasValue && doctorId.Value > 0)
            {
                appointments = appointments.Where(a => a.DoctorId == doctorId.Value);
            }

            if (status.HasValue)
            {
                appointments = appointments.Where(a => a.Status == status.Value);
            }

            if (date.HasValue)
            {
                var selectedDate = date.Value.Date;
                var nextDay = selectedDate.AddDays(1);

                appointments = appointments.Where(a =>
                    a.AppointmentDate >= selectedDate &&
                    a.AppointmentDate < nextDay);
            }

            var result = await appointments
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(result);
        }

        [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        public async Task<IActionResult> Create(int? patientId, int? doctorId)
        {
            if (User.IsInRole("Pacjent"))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

                if (patient == null)
                {
                    return RedirectToAction("Index", "PatientPanel");
                }

                patientId = patient.Id;
            }

            await LoadSelectListsAsync(patientId, doctorId);

            var appointment = new Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(1).Date.AddHours(10),
                Status = AppointmentStatus.Scheduled
            };

            if (patientId.HasValue)
            {
                appointment.PatientId = patientId.Value;
            }

            if (doctorId.HasValue)
            {
                appointment.DoctorId = doctorId.Value;
            }

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (User.IsInRole("Pacjent"))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

                if (patient == null)
                {
                    return RedirectToAction("Index", "PatientPanel");
                }

                appointment.PatientId = patient.Id;
                appointment.Status = AppointmentStatus.Scheduled;
            }

            ValidateAppointmentDate(appointment);

            await ValidateDoctorAvailabilityAsync(appointment);

            if (ModelState.IsValid)
            {
                appointment.CreatedAt = DateTime.Now;

                _context.Add(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Wizyta została umówiona pomyślnie.";

                if (User.IsInRole("Pacjent"))
                {
                    return RedirectToAction("Index", "PatientPanel");
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadSelectListsAsync(appointment.PatientId, appointment.DoctorId);
            return View(appointment);
        }

        [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            await LoadSelectListsAsync(appointment.PatientId, appointment.DoctorId);
            return View(appointment);
        }

        [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            ValidateAppointmentDate(appointment);

            await ValidateDoctorAvailabilityAsync(appointment, appointment.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAppointment = await _context.Appointments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (existingAppointment == null)
                    {
                        return NotFound();
                    }

                    appointment.CreatedAt = existingAppointment.CreatedAt;

                    _context.Update(appointment);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Wizyta została zaktualizowana.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadSelectListsAsync(appointment.PatientId, appointment.DoctorId);
            return View(appointment);
        }

        [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Wizyta została usunięta.";
            return RedirectToAction(nameof(Index));
        }

        private void ValidateAppointmentDate(Appointment appointment)
        {
            if (appointment.AppointmentDate < DateTime.Now && appointment.Status == AppointmentStatus.Scheduled)
            {
                ModelState.AddModelError("AppointmentDate", "Nie można zaplanować wizyty w przeszłości.");
            }

            if (appointment.AppointmentDate.Hour < 7 || appointment.AppointmentDate.Hour > 20)
            {
                ModelState.AddModelError("AppointmentDate", "Wizytę można zaplanować między godziną 07:00 a 20:00.");
            }
        }

        private async Task ValidateDoctorAvailabilityAsync(Appointment appointment, int? ignoredAppointmentId = null)
        {
            var start = appointment.AppointmentDate.AddMinutes(-29);
            var end = appointment.AppointmentDate.AddMinutes(29);

            var conflictQuery = _context.Appointments
                .Where(a =>
                    a.DoctorId == appointment.DoctorId &&
                    a.Status != AppointmentStatus.Cancelled &&
                    a.AppointmentDate >= start &&
                    a.AppointmentDate <= end);

            if (ignoredAppointmentId.HasValue)
            {
                conflictQuery = conflictQuery.Where(a => a.Id != ignoredAppointmentId.Value);
            }

            var hasConflict = await conflictQuery.AnyAsync();

            if (hasConflict)
            {
                ModelState.AddModelError("AppointmentDate", "Ten lekarz ma już wizytę w podobnym terminie. Zachowaj minimum 30 minut odstępu.");
            }
        }

        private async Task LoadSelectListsAsync(int? patientId = null, int? doctorId = null)
        {
            var patients = await _context.Patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new
                {
                    p.Id,
                    FullName = p.LastName + " " + p.FirstName + " — PESEL: " + p.PESEL
                })
                .ToListAsync();

            var doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .Where(d => d.IsActive)
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .Select(d => new
                {
                    d.Id,
                    FullName = "Dr " + d.LastName + " " + d.FirstName + " — " + d.Specialization!.Name
                })
                .ToListAsync();

            ViewBag.Patients = new SelectList(patients, "Id", "FullName", patientId);
            ViewBag.Doctors = new SelectList(doctors, "Id", "FullName", doctorId);

            ViewBag.Statuses = new SelectList(new[]
            {
                new { Id = AppointmentStatus.Scheduled, Name = "Zaplanowana" },
                new { Id = AppointmentStatus.Completed, Name = "Zrealizowana" },
                new { Id = AppointmentStatus.Cancelled, Name = "Odwołana" },
                new { Id = AppointmentStatus.NoShow, Name = "Pacjent nieobecny" }
            }, "Id", "Name");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}