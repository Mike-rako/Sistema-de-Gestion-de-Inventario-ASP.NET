using ExamenMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace ExamenMVC.Models;

public class Movimiento
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Required, RegularExpression("Entrada|Salida")]
    public string Tipo { get; set; } = "Entrada";

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser > 0")]
    public int Cantidad { get; set; }

    [StringLength(200)]
    public string? Observacion { get; set; }

    // FK
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
}
