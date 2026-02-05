using System.ComponentModel.DataAnnotations;
using ExamenMVC.Models;

namespace ExamenMVC.Models.ViewModels
{
    public class MovimientoCreateVM
    {
        // --- Paso Buscar ---
        public string? Q { get; set; }                       // texto de búsqueda
        public List<Producto> Resultados { get; set; } = new(); // resultados a mostrar

        // --- Paso Seleccionar ---
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }                  // id del producto elegido
        public string? ProductoResumen { get; set; }         // texto resumen del seleccionado

        // --- Paso Registrar ---
        [Required]
        [RegularExpression("Entrada|Salida")]
        public string Tipo { get; set; } = "Entrada";

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser > 0")]
        public int Cantidad { get; set; }

        [StringLength(200)]
        public string? Observacion { get; set; }
    }
}
