using ClothesWeb.Models;
using Microsoft.EntityFrameworkCore;
using ClothesWeb.Models;

namespace ClothesWeb;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    
}