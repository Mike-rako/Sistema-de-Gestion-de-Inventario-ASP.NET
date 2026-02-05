using System.ComponentModel.DataAnnotations;
using ExamenMVC.Models;

namespace ExamenMVC.Models.ViewModels;

public class ReporteMovimientosVM
{
    [DataType(DataType.Date)]
    public DateTime? Desde { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Hasta { get; set; }

    // "", "Entrada" o "Salida"
    public string? Tipo { get; set; } = "";

    public List<Movimiento> Resultados { get; set; } = new();
    public int TotalEntradas { get; set; }
    public int TotalSalidas { get; set; }
    public int Balance { get; set; } // Entradas - Salidas
}
