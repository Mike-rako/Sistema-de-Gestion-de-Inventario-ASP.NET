using ExamenMVC.Data;
using ExamenMVC.Models;
using ExamenMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExamenMVC.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _ctx;
        public UsuariosController(AppDbContext ctx) => _ctx = ctx;

        [HttpGet]
        public IActionResult Create() => View(new UsuarioCreateVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var exists = await _ctx.Usuarios.AnyAsync(u => u.UserName == vm.UserName);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.UserName), "Ese usuario ya existe.");
                return View(vm);
            }

            var usuario = new Usuario
            {
                UserName = vm.UserName,
                PasswordHash = Sha256(vm.Password),
                Role = string.IsNullOrWhiteSpace(vm.Role) ? "User" : vm.Role
            };

            _ctx.Usuarios.Add(usuario);
            await _ctx.SaveChangesAsync();

            TempData["ok"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Create));
        }

        private static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return System.Convert.ToHexString(bytes).ToLower();
        }
    }
}
