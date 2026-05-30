using ClinicSystem.Data;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    [Authorize(Roles = "Administrator,Recepcjonista,Lekarz")]
    public class SpecializationsController : Controller
    {
        private readonly ClinicDbContext _context;

        public SpecializationsController(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var specializations = _context.Specializations
                .Include(s => s.Doctors)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                specializations = specializations.Where(s =>
                    s.Name.Contains(searchString) ||
                    (s.Description != null && s.Description.Contains(searchString)));
            }

            var result = await specializations
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations
                .Include(s => s.Doctors)
                    .ThenInclude(d => d.Appointments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        public IActionResult Create()
        {
            return View(new Specialization());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specialization specialization)
        {
            if (await _context.Specializations.AnyAsync(s => s.Name == specialization.Name))
            {
                ModelState.AddModelError("Name", "Specjalizacja o takiej nazwie już istnieje.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(specialization);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Specjalizacja została dodana.";
                return RedirectToAction(nameof(Index));
            }

            return View(specialization);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations.FindAsync(id);

            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Specialization specialization)
        {
            if (id != specialization.Id)
            {
                return NotFound();
            }

            if (await _context.Specializations.AnyAsync(s => s.Name == specialization.Name && s.Id != specialization.Id))
            {
                ModelState.AddModelError("Name", "Inna specjalizacja ma już taką nazwę.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialization);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Specjalizacja została zaktualizowana.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecializationExists(specialization.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(specialization);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations
                .Include(s => s.Doctors)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialization = await _context.Specializations
                .Include(s => s.Doctors)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialization == null)
            {
                return NotFound();
            }

            if (specialization.Doctors.Any())
            {
                TempData["ErrorMessage"] = "Nie można usunąć specjalizacji, ponieważ są do niej przypisani lekarze.";
                return RedirectToAction(nameof(Delete), new { id = specialization.Id });
            }

            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Specjalizacja została usunięta.";
            return RedirectToAction(nameof(Index));
        }

        private bool SpecializationExists(int id)
        {
            return _context.Specializations.Any(e => e.Id == id);
        }
    }
}