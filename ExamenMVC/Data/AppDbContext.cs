using Microsoft.EntityFrameworkCore;
using ExamenMVC.Models;


namespace ExamenMVC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Unicidad
        mb.Entity<Usuario>().HasIndex(u => u.UserName).IsUnique();
        mb.Entity<Producto>().HasIndex(p => p.Codigo).IsUnique();

        // Precisión y checks
        mb.Entity<Producto>().Property(p => p.Precio).HasColumnType("decimal(10,2)");
        mb.Entity<Producto>().HasCheckConstraint("CK_Producto_Stock", "[Stock] >= 0");
        mb.Entity<Producto>().HasCheckConstraint("CK_Producto_Precio", "[Precio] >= 0");

        // Relación Movimiento -> Producto (requerida)
        mb.Entity<Movimiento>()
          .HasOne(m => m.Producto)
          .WithMany(p => p.Movimientos)
          .HasForeignKey(m => m.ProductoId)
          .OnDelete(DeleteBehavior.Cascade);

        // Seed de un usuario (user: admin / pass: admin123)
        mb.Entity<Usuario>().HasData(new Usuario
        {
            Id = 1,
            UserName = "admin",
            // SHA256("admin123")
            PasswordHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9",
            Role = "Admin"
        });
    }
}
