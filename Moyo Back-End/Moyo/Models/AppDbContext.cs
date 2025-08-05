using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Moyo.Models;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Vendor> Vendors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
           
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ORDERS__3214EC27D7EEEB6D");

            entity.ToTable("ORDERS");

            entity.HasIndex(e => e.OrderNumber, "UQ__ORDERS__CAC5E743504D33E3").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ORDER_ID__3214EC27CC66E4A4");

            entity.ToTable("ORDER_ITEMS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("QUANTITY");

            entity.HasOne(d => d.Order).WithMany(o => o.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ORDER_ITEM__Order__48CFD27E");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ORDER_ITE__Produ__49C3F6B7");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCTS__3214EC27823ACBC7");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.Id).HasColumnName("ID")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Description)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Name)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.ProductCategoryId).HasColumnName("ProductCategoryId");
            entity.Property(e => e.VendorId).HasColumnName("VendorId");

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(5,2)")
                .HasColumnName("PRICE");

            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasColumnName("QUANTITY");

            entity.Property(e => e.Image)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("IMAGE");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasColumnName("IsActive");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCT___3214EC273C86C47F");

            entity.ToTable("PRODUCT_CATEGORIES");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Name)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("AspNetUsers");

            entity.Property(u => u.Name).IsRequired().HasMaxLength(225);
            entity.Property(u => u.Surname).IsRequired().HasMaxLength(225);
            entity.Property(u => u.Address).IsRequired().HasMaxLength(225);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.Property(u => u.UserName).IsRequired().HasMaxLength(256);
            entity.Property(u => u.NormalizedUserName).HasMaxLength(256);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.NormalizedEmail).HasMaxLength(256);
            entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(10);
        });

        modelBuilder.Entity<IdentityUserRole<int>>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
        });

        modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
        });

        modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
            entity.HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });
        });

        modelBuilder.Entity<IdentityUserToken<int>>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
            entity.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VENDORS__3214EC27E0B44A4F");

            entity.ToTable("VENDORS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Email)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Name)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUMBER");
        });

    }
}
