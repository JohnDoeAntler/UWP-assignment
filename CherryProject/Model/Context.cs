﻿using System;
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
        public virtual DbSet<Dic> Dic { get; set; }
        public virtual DbSet<Did> Did { get; set; }
        public virtual DbSet<DidSpare> DidSpare { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderProduct> OrderProduct { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Spare> Spare { get; set; }
        public virtual DbSet<User> User { get; set; }

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

            modelBuilder.Entity<Dic>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("OrderId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.LastTimeModified)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Dic)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("Dic_ibfk_1");
            });

            modelBuilder.Entity<Did>(entity =>
            {
                entity.HasIndex(e => e.DicId)
                    .HasName("DicId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.DicId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LastTimeModified)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Quantity).HasColumnType("int(11)");

                entity.HasOne(d => d.Dic)
                    .WithMany(p => p.InverseDic)
                    .HasForeignKey(d => d.DicId)
                    .HasConstraintName("Did_ibfk_1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Did)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("Did_ibfk_2");
            });

            modelBuilder.Entity<DidSpare>(entity =>
            {
                entity.HasKey(e => new { e.DidId, e.SpareId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.SpareId)
                    .HasName("SpareId");

                entity.Property(e => e.DidId).HasColumnType("varchar(255)");

                entity.Property(e => e.SpareId).HasColumnType("varchar(255)");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.HasOne(d => d.Did)
                    .WithMany(p => p.DidSpare)
                    .HasForeignKey(d => d.DidId)
                    .HasConstraintName("DidSpare_ibfk_1");

                entity.HasOne(d => d.Spare)
                    .WithMany(p => p.DidSpare)
                    .HasForeignKey(d => d.SpareId)
                    .HasConstraintName("DidSpare_ibfk_2");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasIndex(e => e.DidId)
                    .HasName("OrderId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.DidId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Did)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.DidId)
                    .HasConstraintName("Didid_FK");
            });

            modelBuilder.Entity<Order>(entity =>
            {
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

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.Dealer)
                    .WithMany(p => p.OrderDealer)
                    .HasForeignKey(d => d.DealerId)
                    .HasConstraintName("UserId_FK");

                entity.HasOne(d => d.Modifier)
                    .WithMany(p => p.OrderModifier)
                    .HasForeignKey(d => d.ModifierId)
                    .HasConstraintName("Order_ibfk_1");
			});

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PRIMARY");

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
                    .WithMany(p => p.OrderProduct)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("OrderProduct_ibfk_1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProduct)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("OrderProduct_ibfk_2");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("Name")
                    .IsUnique();

                entity.HasIndex(e => e.NormalizedName)
                    .HasName("NormalizedName_2")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
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

				entity.Property(e => e.IconUrl).HasColumnType("varchar(2083)");

				entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.Description).HasColumnType("longtext");

                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(10,0)")
                    .HasDefaultValueSql("'0.000'");

                entity.Property(e => e.Duration).HasColumnType("bigint(20)");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

				entity.Property(e => e.ImageUrl).HasColumnType("varchar(2083)");

				entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Promotion)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("ProductId_FK2");
            });

            modelBuilder.Entity<Role>(entity =>
            {
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
                entity.HasIndex(e => e.CategoryId)
                    .HasName("CATEGORYID")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PositionXoffset)
                    .HasColumnName("PositionXOffset")
                    .HasColumnType("decimal(11,11)");

                entity.Property(e => e.PositionYoffset)
                    .HasColumnName("PositionYOffset")
                    .HasColumnType("decimal(11,11)");

                entity.HasOne(d => d.Category)
                    .WithOne(p => p.Spare)
                    .HasForeignKey<Spare>(d => d.CategoryId)
                    .HasConstraintName("CategoryId_FK");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("Email")
                    .IsUnique();

                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("NormalizedEmail")
                    .IsUnique();

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

                entity.HasIndex(e => e.UserName)
                    .HasName("UserName")
                    .IsUnique();

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

				entity.Property(e => e.IconUrl).HasColumnType("varchar(2083)");

				entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("RoleId_FK");
            });
        }
    }
}
