using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamenMVC.Data;
using ExamenMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace ExamenMVC.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Productos?q=texto
        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.Productos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qLower = q.Trim().ToLower();
                query = query.Where(p =>
                    p.Codigo.ToLower().Contains(qLower) ||
                    p.Nombre.ToLower().Contains(qLower) ||
                    p.Categoria.ToLower().Contains(qLower)
                );
            }

            var lista = await query.ToListAsync();
            ViewData["q"] = q; // para rellenar la caja de búsqueda
            return View(lista);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();
            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create() => View();

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            // Regla de negocio: si hay stock, precio > 0
            if (producto.Stock > 0 && producto.Precio <= 0)
                ModelState.AddModelError(nameof(producto.Precio), "Si hay stock, el precio debe ser mayor que cero.");

            // Código único
            if (await _context.Productos.AnyAsync(p => p.Codigo == producto.Codigo))
                ModelState.AddModelError(nameof(producto.Codigo), "Ya existe un producto con ese código.");

            if (!ModelState.IsValid) return View(producto);

            try
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["ok"] = "Producto creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Por si la BD tiene índice único y lanza excepción
                ModelState.AddModelError(nameof(producto.Codigo), "El código ya está en uso.");
                return View(producto);
            }
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest();

            // Regla de negocio: si hay stock, precio > 0
            if (producto.Stock > 0 && producto.Precio <= 0)
                ModelState.AddModelError(nameof(producto.Precio), "Si hay stock, el precio debe ser mayor que cero.");

            // Código único (excluyendo el propio Id)
            if (await _context.Productos.AnyAsync(p => p.Codigo == producto.Codigo && p.Id != id))
                ModelState.AddModelError(nameof(producto.Codigo), "Ya existe otro producto con ese código.");

            if (!ModelState.IsValid) return View(producto);

            try
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["ok"] = "Producto actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var existe = await _context.Productos.AnyAsync(e => e.Id == producto.Id);
                if (!existe) return NotFound();
                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(producto.Codigo), "El código ya está en uso.");
                return View(producto);
            }
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Bloquear eliminación si hay movimientos vinculados
            bool tieneMovs = await _context.Movimientos.AnyAsync(m => m.ProductoId == id);
            if (tieneMovs)
            {
                TempData["error"] = "No se puede eliminar: el producto tiene movimientos registrados.";
                return RedirectToAction(nameof(Index));
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["ok"] = "Producto eliminado.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
