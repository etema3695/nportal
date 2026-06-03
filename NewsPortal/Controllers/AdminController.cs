using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsPortal.Models;
using NewsPortal.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var usersWithRoles = (from user in _context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      RoleNames = (from userRole in _context.UserRoles
                                                   where userRole.UserId == user.Id
                                                   join role in _context.Roles on userRole.RoleId equals role.Id
                                                   select role.Name).ToList()
                                  })
                                  .ToList()
                                  .Select(p => new UserInRoleViewModel
                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Role = string.Join(",", p.RoleNames)
                                  });

            return View(usersWithRoles);
        }

        public IActionResult CreateUser()
        {
            ViewBag.Roles = _context.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(string txtEmail, string roleName, string txtPassword, string txtNumber)
        {
            var user = new ApplicationUser
            {
                UserName = txtEmail,
                Email = txtEmail,
                Phone = txtNumber,
                PhoneNumber = txtNumber
            };

            var result = await _userManager.CreateAsync(user, txtPassword);
            if (result.Succeeded && !string.IsNullOrWhiteSpace(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Roles = _context.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
            return View();
        }
    }
}
