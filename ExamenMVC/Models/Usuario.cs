using System.ComponentModel.DataAnnotations;

namespace ExamenMVC.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string PasswordHash { get; set; } = string.Empty; // SHA256 hex

    [StringLength(20)]
    public string? Role { get; set; } = "User";
}
