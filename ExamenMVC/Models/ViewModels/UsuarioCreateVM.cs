using System.ComponentModel.DataAnnotations;

namespace ExamenMVC.Models.ViewModels;

public class UsuarioCreateVM
{
    [Required, StringLength(50)]
    [Display(Name = "Usuario")]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Rol")]
    public string? Role { get; set; } = "User";
}
