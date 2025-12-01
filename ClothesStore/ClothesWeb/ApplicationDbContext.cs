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
    public DbSet<Category> Category { get; set; }


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

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SellItem>()
            .HasOne(si => si.Sell)
            .WithMany(s => s.SellItem) 
            .HasForeignKey(si => si.SellId);

        modelBuilder.Entity<SellItem>()
            .HasOne(si => si.Product)
            .WithMany() 
            .HasForeignKey(si => si.ProductId);


        modelBuilder.Entity<SellItem>()
            .HasOne(si => si.Size)
            .WithMany() 
            .HasForeignKey(si => si.SizeId);

    }

}