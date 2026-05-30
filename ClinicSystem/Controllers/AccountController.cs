using ClinicSystem.Data;
using ClinicSystem.Models;
using ClinicSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ClinicDbContext _context;


        public AccountController(
               UserManager<ApplicationUser> userManager,
               SignInManager<ApplicationUser> signInManager,
               ClinicDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy e-mail lub hasło.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Nieprawidłowy e-mail lub hasło.");
            return View(model);
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _context.Patients.AnyAsync(p => p.PESEL == model.Pesel))
            {
                ModelState.AddModelError("Pesel", "Pacjent z takim numerem PESEL już istnieje.");
            }

            if (model.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Data urodzenia nie może być z przyszłości.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Pacjent");

                var patient = new Patient
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PESEL = model.Pesel,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Address = model.Address,
                    CreatedAt = DateTime.Now,
                    ApplicationUserId = user.Id
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["SuccessMessage"] = "Konto pacjenta zostało utworzone. Możesz teraz korzystać ze strony przychodni.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, TranslateIdentityError(error.Code, error.Description));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private string TranslateIdentityError(string code, string description)
        {
            return code switch
            {
                "DuplicateUserName" => "Użytkownik z takim adresem e-mail już istnieje.",
                "DuplicateEmail" => "Użytkownik z takim adresem e-mail już istnieje.",
                "PasswordTooShort" => "Hasło jest za krótkie.",
                "PasswordRequiresDigit" => "Hasło musi zawierać przynajmniej jedną cyfrę.",
                "PasswordRequiresLower" => "Hasło musi zawierać małą literę.",
                "PasswordRequiresUpper" => "Hasło musi zawierać wielką literę.",
                _ => description
            };
        }
    }
}