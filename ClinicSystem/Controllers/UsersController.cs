using ClinicSystem.Models;
using ClinicSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Email!.Contains(searchString) ||
                    (u.FirstName != null && u.FirstName.Contains(searchString)) ||
                    (u.LastName != null && u.LastName.Contains(searchString)));
            }

            var users = await usersQuery
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CurrentRole = roles.FirstOrDefault() ?? "Brak roli",
                    SelectedRole = roles.FirstOrDefault() ?? "",
                    CreatedAt = user.CreatedAt
                });
            }

            ViewBag.Roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name!)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                CurrentRole = roles.FirstOrDefault() ?? "Brak roli",
                SelectedRole = roles.FirstOrDefault() ?? "",
                CreatedAt = user.CreatedAt
            };

            ViewBag.Roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name!)
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(model.SelectedRole))
            {
                ModelState.AddModelError("SelectedRole", "Wybierz rolę użytkownika.");
            }

            if (!await _roleManager.RoleExistsAsync(model.SelectedRole))
            {
                ModelState.AddModelError("SelectedRole", "Wybrana rola nie istnieje.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _roleManager.Roles
                    .OrderBy(r => r.Name)
                    .Select(r => r.Name!)
                    .ToListAsync();

                model.Email = user.Email ?? "";
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.CreatedAt = user.CreatedAt;

                var currentRoles = await _userManager.GetRolesAsync(user);
                model.CurrentRole = currentRoles.FirstOrDefault() ?? "Brak roli";

                return View(model);
            }

            var oldRoles = await _userManager.GetRolesAsync(user);

            if (oldRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
            }

            await _userManager.AddToRoleAsync(user, model.SelectedRole);

            TempData["SuccessMessage"] = "Rola użytkownika została zmieniona.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                CurrentRole = roles.FirstOrDefault() ?? "Brak roli",
                SelectedRole = roles.FirstOrDefault() ?? "",
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }
    }
}