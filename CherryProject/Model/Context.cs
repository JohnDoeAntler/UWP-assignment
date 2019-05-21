using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CherryProject.Model
{
    public partial class Context : DbContext
    {
        public Context()
        {
        }

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<Orderproduct> Orderproduct { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Promotions> Promotions { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Spare> Spare { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               optionsBuilder.UseMySql("server=colourful.dlinkddns.com;uid=system;pwd=F10gNfXZg6sIvBkP;database=system_project");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.HasIndex(e => e.NormalizedName)
                    .HasName("NormalizedName");

                entity.HasIndex(e => e.ProductId)
                    .HasName("productId_FK");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("productId_FK");
            });

            modelBuilder.Entity<Invoices>(entity =>
            {
                entity.ToTable("invoices");

                entity.HasIndex(e => e.OrderId)
                    .HasName("OrderId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("OrderId_FK");
            });

            modelBuilder.Entity<Orderproduct>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PRIMARY");

                entity.ToTable("orderproduct");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.OrderId).HasColumnType("varchar(255)");

                entity.Property(e => e.ProductId).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.LastTimeModified)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Quantity).HasColumnType("int(11)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Orderproduct)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("orderproduct_ibfk_1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Orderproduct)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("orderproduct_ibfk_2");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.ToTable("orders");

                entity.HasIndex(e => e.DealerId)
                    .HasName("USER")
                    .IsUnique();

                entity.HasIndex(e => e.ModifierId)
                    .HasName("ModifierId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.DealerId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LastTimeModified)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.ModifierId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.Dealer)
                    .WithOne(p => p.Orders)
                    .HasForeignKey<Orders>(d => d.DealerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserId_FK");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.ToTable("products");

                entity.HasIndex(e => e.Name)
                    .HasName("Name")
                    .IsUnique();

                entity.HasIndex(e => e.NormalizedName)
                    .HasName("NormalizedName_2")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.DangerLevel).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.LastTimeModified)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.Weight).HasColumnType("double(3,3)");
            });

            modelBuilder.Entity<Promotions>(entity =>
            {
                entity.ToTable("promotions");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.Description).HasColumnType("longtext");

                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(3,3)")
                    .HasDefaultValueSql("'0.000'");

                entity.Property(e => e.Duration).HasColumnType("bigint(20)");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("ProductId_FK2");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("roles");

                entity.HasIndex(e => e.Name)
                    .HasName("Name")
                    .IsUnique();

                entity.HasIndex(e => e.NormalizedName)
                    .HasName("NormalizedName");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp).HasColumnType("longtext");

                entity.Property(e => e.Name).HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedName).HasColumnType("varchar(256)");
            });

            modelBuilder.Entity<Spare>(entity =>
            {
                entity.ToTable("spare");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("CATEGORYID")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.PositionXoffset)
                    .HasColumnName("PositionXOffset")
                    .HasColumnType("decimal(11,11)");

                entity.Property(e => e.PositionYoffset)
                    .HasColumnName("PositionYOffset")
                    .HasColumnType("decimal(11,11)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.Category)
                    .WithOne(p => p.Spare)
                    .HasForeignKey<Spare>(d => d.CategoryId)
                    .HasConstraintName("CategoryId_FK");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedFirstName)
                    .HasName("NormalizedFirstName");

                entity.HasIndex(e => e.NormalizedLastName)
                    .HasName("NormalizedLastName");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.HasIndex(e => e.Region)
                    .HasName("Region");

                entity.HasIndex(e => e.RoleId)
                    .HasName("Users_RoleId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.Address).HasColumnType("longtext");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.EmailConfirmed).HasColumnType("bit(1)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedEmail)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedFirstName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedLastName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedUserName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.PhoneNumberConfirmed).HasColumnType("bit(1)");

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SecurityStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("RoleId_FK");
            });
        }
    }
}
