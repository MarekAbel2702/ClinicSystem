using ClinicSystem.Data;
using ClinicSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClinicDbContext _context;

        public HomeController(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomePageViewModel
            {
                PatientsCount = await _context.Patients.CountAsync(),
                DoctorsCount = await _context.Doctors.CountAsync(d => d.IsActive),
                SpecializationsCount = await _context.Specializations.CountAsync(),
                AppointmentsCount = await _context.Appointments.CountAsync(),

                FeaturedDoctors = await _context.Doctors
                    .Include(d => d.Specialization)
                    .Where(d => d.IsActive)
                    .OrderBy(d => d.LastName)
                    .Take(6)
                    .ToListAsync(),

                Specializations = await _context.Specializations
                    .Include(s => s.Doctors)
                    .OrderBy(s => s.Name)
                    .Take(8)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}