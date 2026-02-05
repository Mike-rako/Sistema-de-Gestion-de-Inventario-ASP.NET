using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ExamenMVC.Data;

namespace ExamenMVC.Services;

public interface IAuthService
{
    Task<int?> ValidateAsync(string userName, string password);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _ctx;
    public AuthService(AppDbContext ctx) => _ctx = ctx;

    public async Task<int?> ValidateAsync(string userName, string password)
    {
        var hash = Sha256(password);
        var user = await _ctx.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName && u.PasswordHash == hash);
        return user?.Id;
    }

    private static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower(); // coincide con el seed
    }
}
