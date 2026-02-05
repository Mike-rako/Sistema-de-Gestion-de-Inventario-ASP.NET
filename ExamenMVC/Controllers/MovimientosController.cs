using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamenMVC.Data;
using ExamenMVC.Models;
using ExamenMVC.Models.ViewModels;
using System.Linq;

namespace ExamenMVC.Controllers
{
    [Authorize]
    public class MovimientosController : Controller
    {
        private readonly AppDbContext _ctx;
        public MovimientosController(AppDbContext ctx) => _ctx = ctx;

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var data = await _ctx.Movimientos
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
            return View(data);
        }

        // --- FORZAR FLUJO: BUSCAR -> SELECCIONAR -> REGISTRAR ---

        // GET: Create (busca y/o selecciona un producto antes de registrar)
        // params:
        //   q: texto de búsqueda
        //   selectId: id del producto seleccionado desde los resultados
        [HttpGet]
        public async Task<IActionResult> Create(string? q, int? selectId)
        {
            var vm = new MovimientoCreateVM { Q = q };

            // Si viene selección, fijar el producto elegido
            if (selectId.HasValue)
            {
                var sel = await _ctx.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == selectId.Value);
                if (sel != null)
                {
                    vm.ProductoId = sel.Id;
                    vm.ProductoResumen = $"{sel.Codigo} - {sel.Nombre} (Stock: {sel.Stock})";
                }
            }

            // Si hay texto de búsqueda, mostrar resultados (código/nombre/categoría)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var qLower = q.Trim().ToLower();
                vm.Resultados = await _ctx.Productos.AsNoTracking()
                    .Where(p =>
                        p.Codigo.ToLower().Contains(qLower) ||
                        p.Nombre.ToLower().Contains(qLower) ||
                        p.Categoria.ToLower().Contains(qLower))
                    .OrderBy(p => p.Nombre)
                    .Take(25)
                    .ToListAsync();
            }

            return View(vm); // la vista controlará los 3 pasos
        }

        // POST: registrar movimiento (solo si hay un producto seleccionado)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimientoCreateVM vm)
        {
            // Validación: debe haber producto elegido
            if (vm.ProductoId <= 0)
            {
                ModelState.AddModelError(nameof(vm.ProductoId), "Primero busque y seleccione un producto.");
            }

            // Reconstruir resumen del producto (por si hay error y volvemos a la vista)
            if (vm.ProductoId > 0)
            {
                var sel = await _ctx.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == vm.ProductoId);
                if (sel != null)
                    vm.ProductoResumen = $"{sel.Codigo} - {sel.Nombre} (Stock: {sel.Stock})";
            }

            if (!ModelState.IsValid)
            {
                // Si rebotamos, opcional: mostrar otra vez resultados de la última búsqueda
                if (!string.IsNullOrWhiteSpace(vm.Q))
                {
                    var qLower = vm.Q.Trim().ToLower();
                    vm.Resultados = await _ctx.Productos.AsNoTracking()
                        .Where(p =>
                            p.Codigo.ToLower().Contains(qLower) ||
                            p.Nombre.ToLower().Contains(qLower) ||
                            p.Categoria.ToLower().Contains(qLower))
                        .OrderBy(p => p.Nombre)
                        .Take(25)
                        .ToListAsync();
                }
                return View(vm);
            }

            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var prod = await _ctx.Productos.FirstOrDefaultAsync(p => p.Id == vm.ProductoId);
                if (prod is null)
                {
                    ModelState.AddModelError(nameof(vm.ProductoId), "Producto no encontrado.");
                    return View(vm);
                }

                if (vm.Tipo == "Entrada")
                {
                    prod.Stock += vm.Cantidad;
                }
                else if (vm.Tipo == "Salida")
                {
                    if (prod.Stock < vm.Cantidad)
                    {
                        ModelState.AddModelError(nameof(vm.Cantidad), "No hay stock suficiente para la salida.");
                        return View(vm);
                    }
                    prod.Stock -= vm.Cantidad;
                }
                else
                {
                    ModelState.AddModelError(nameof(vm.Tipo), "Tipo inválido (Entrada|Salida).");
                    return View(vm);
                }

                var mov = new Movimiento
                {
                    ProductoId = vm.ProductoId,
                    Tipo = vm.Tipo,
                    Cantidad = vm.Cantidad,
                    Observacion = vm.Observacion,
                    Fecha = DateTime.UtcNow
                };

                _ctx.Movimientos.Add(mov);
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["ok"] = "Movimiento registrado y stock actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Error al guardar el movimiento.");
                return View(vm);
            }
        }

        // REPORTE (igual que antes)
        [HttpGet]
        public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta, string? tipo = "")
        {
            var q = _ctx.Movimientos
                .Include(m => m.Producto)
                .AsQueryable();

            if (desde.HasValue) q = q.Where(m => m.Fecha >= desde.Value.Date);
            if (hasta.HasValue) q = q.Where(m => m.Fecha < hasta.Value.Date.AddDays(1));
            if (!string.IsNullOrWhiteSpace(tipo)) q = q.Where(m => m.Tipo == tipo);

            var lista = await q.OrderByDescending(m => m.Fecha).ToListAsync();

            var vm = new ReporteMovimientosVM
            {
                Desde = desde,
                Hasta = hasta,
                Tipo = tipo ?? "",
                Resultados = lista,
                TotalEntradas = lista.Where(m => m.Tipo == "Entrada").Sum(m => m.Cantidad),
                TotalSalidas = lista.Where(m => m.Tipo == "Salida").Sum(m => m.Cantidad)
            };
            vm.Balance = vm.TotalEntradas - vm.TotalSalidas;

            return View(vm);
        }
    }
}
