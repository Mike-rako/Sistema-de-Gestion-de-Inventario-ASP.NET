using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ExamenMVC.Models.ViewModels;
using ExamenMVC.Services;
using ExamenMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ExamenMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _auth;
        private readonly AppDbContext _ctx;

        public LoginController(IAuthService auth, AppDbContext ctx)
        {
            _auth = auth;
            _ctx = ctx;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index() => View(new LoginVM());

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = await _auth.ValidateAsync(vm.UserName, vm.Password);
            if (userId is null)
            {
                vm.Error = "Usuario o contraseña incorrectos.";
                return View(vm);
            }

            var user = await _ctx.Usuarios.AsNoTracking().FirstAsync(u => u.Id == userId.Value);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Productos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
