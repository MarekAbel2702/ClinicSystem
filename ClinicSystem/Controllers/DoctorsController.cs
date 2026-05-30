using ClinicSystem.Data;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
    public class DoctorsController : Controller
    {
        private readonly ClinicDbContext _context;

        public DoctorsController(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString, int? specializationId)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSpecialization"] = specializationId;

            await LoadSpecializationsAsync();

            var doctors = _context.Doctors
                .Include(d => d.Specialization)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                doctors = doctors.Where(d =>
                    d.FirstName.Contains(searchString) ||
                    d.LastName.Contains(searchString) ||
                    d.LicenseNumber.Contains(searchString) ||
                    (d.PhoneNumber != null && d.PhoneNumber.Contains(searchString)) ||
                    (d.Email != null && d.Email.Contains(searchString)) ||
                    (d.OfficeNumber != null && d.OfficeNumber.Contains(searchString)));
            }

            if (specializationId.HasValue && specializationId.Value > 0)
            {
                doctors = doctors.Where(d => d.SpecializationId == specializationId.Value);
            }

            var result = await doctors
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        public async Task<IActionResult> Create()
        {
            await LoadSpecializationsAsync();

            var doctor = new Doctor
            {
                IsActive = true
            };

            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (await _context.Doctors.AnyAsync(d => d.LicenseNumber == doctor.LicenseNumber))
            {
                ModelState.AddModelError("LicenseNumber", "Lekarz z takim numerem PWZ już istnieje.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Lekarz został dodany pomyślnie.";
                return RedirectToAction(nameof(Index));
            }

            await LoadSpecializationsAsync(doctor.SpecializationId);
            return View(doctor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            await LoadSpecializationsAsync(doctor.SpecializationId);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (await _context.Doctors.AnyAsync(d => d.LicenseNumber == doctor.LicenseNumber && d.Id != doctor.Id))
            {
                ModelState.AddModelError("LicenseNumber", "Inny lekarz ma już taki numer PWZ.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Dane lekarza zostały zaktualizowane.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadSpecializationsAsync(doctor.SpecializationId);
            return View(doctor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            if (doctor.Appointments.Any())
            {
                TempData["ErrorMessage"] = "Nie można usunąć lekarza, ponieważ ma przypisane wizyty.";
                return RedirectToAction(nameof(Delete), new { id = doctor.Id });
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Lekarz został usunięty.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSpecializationsAsync(int? selectedId = null)
        {
            var specializations = await _context.Specializations
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Specializations = new SelectList(specializations, "Id", "Name", selectedId);
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}