using ClothesWeb.Models;
using Microsoft.EntityFrameworkCore;
using ClothesWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ClothesWeb;
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }



    public DbSet<Product> Products { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<Supplier> Supplier { get; set; }
    public DbSet<ProductSizes> ProductSizes { get; set; }

    public DbSet<Sell> Sells { get; set; }
    public DbSet<SellItem> SellItems { get; set; }
    public DbSet<SellComposition> SellCompositions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProductSizes>()
            .HasKey(ps => new { ps.ProductId, ps.SizeId });

        
        modelBuilder.Entity<ProductSizes>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSizes)
            .HasForeignKey(ps => ps.ProductId);

        
        modelBuilder.Entity<ProductSizes>()
            .HasOne(ps => ps.Size)
            .WithMany(s => s.ProductSizes)
            .HasForeignKey(ps => ps.SizeId);

        modelBuilder.Entity<Product>()
        .HasOne(p => p.Supplier)
        .WithMany(s => s.Products)
        .HasForeignKey(p => p.SupplierId)
        .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SellComposition>()
            .HasKey(sc => new { sc.SellId, sc.SellItemId });

        modelBuilder.Entity<SellComposition>()
            .HasOne(sc => sc.Sell)
            .WithMany(s => s.SaleComposition)
            .HasForeignKey(sc => sc.SellId);


        modelBuilder.Entity<SellComposition>()
            .HasOne(sc => sc.SellItem)
            .WithMany()
            .HasForeignKey(sc => sc.SellItemId);
    }

}