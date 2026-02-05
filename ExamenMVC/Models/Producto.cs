using System.ComponentModel.DataAnnotations;

namespace ExamenMVC.Models;

public class Producto
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string Codigo { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Range(0, 9999999)]
    public decimal Precio { get; set; }

    [Required, StringLength(60)]
    public string Categoria { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Stock { get; set; } = 0;

    public bool Activo { get; set; } = true;

    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
