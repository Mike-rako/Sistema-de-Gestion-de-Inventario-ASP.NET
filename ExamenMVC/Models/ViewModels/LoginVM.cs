using System.ComponentModel.DataAnnotations;

namespace ExamenMVC.Models.ViewModels;

public class LoginVM
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? Error { get; set; }
}
