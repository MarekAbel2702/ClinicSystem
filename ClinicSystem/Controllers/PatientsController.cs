using ClinicSystem.Data;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
    public class PatientsController : Controller
    {
        private readonly ClinicDbContext _context;

        public PatientsController(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var patients = _context.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                patients = patients.Where(p =>
                    p.FirstName.Contains(searchString) ||
                    p.LastName.Contains(searchString) ||
                    p.PESEL.Contains(searchString) ||
                    (p.PhoneNumber != null && p.PhoneNumber.Contains(searchString)) ||
                    (p.Email != null && p.Email.Contains(searchString)));
            }

            var result = await patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        public IActionResult Create()
        {
            var patient = new Patient
            {
                DateOfBirth = DateTime.Today.AddYears(-18)
            };

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (await _context.Patients.AnyAsync(p => p.PESEL == patient.PESEL))
            {
                ModelState.AddModelError("Pesel", "Pacjent z takim numerem PESEL już istnieje.");
            }

            if (patient.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Data urodzenia nie może być z przyszłości.");
            }

            if (ModelState.IsValid)
            {
                patient.CreatedAt = DateTime.Now;

                _context.Add(patient);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pacjent został dodany pomyślnie.";
                return RedirectToAction(nameof(Index));
            }

            return View(patient);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (await _context.Patients.AnyAsync(p => p.PESEL == patient.PESEL && p.Id != patient.Id))
            {
                ModelState.AddModelError("Pesel", "Inny pacjent ma już taki numer PESEL.");
            }

            if (patient.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Data urodzenia nie może być z przyszłości.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPatient = await _context.Patients.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingPatient == null)
                    {
                        return NotFound();
                    }

                    patient.CreatedAt = existingPatient.CreatedAt;

                    _context.Update(patient);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Dane pacjenta zostały zaktualizowane.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(patient);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            if (patient.Appointments.Any())
            {
                TempData["ErrorMessage"] = "Nie można usunąć pacjenta, ponieważ ma przypisane wizyty.";
                return RedirectToAction(nameof(Delete), new { id = patient.Id });
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pacjent został usunięty.";
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}