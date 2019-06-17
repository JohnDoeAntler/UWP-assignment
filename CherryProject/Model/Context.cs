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
        public virtual DbSet<Dic> Dic { get; set; }
        public virtual DbSet<Did> Did { get; set; }
        public virtual DbSet<DidSpare> DidSpare { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderProduct> OrderProduct { get; set; }
        public virtual DbSet<PriceHistory> PriceHistory { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
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
                    .HasColumnType("enum('Dispatching','Dispatched','Accepted')");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Dic)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("Dic_ibfk_1");
            });

            modelBuilder.Entity<Did>(entity =>
            {
                entity.HasIndex(e => e.DicId)
                    .HasName("Did_ibfk_1");

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
                    .WithMany(p => p.Did)
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
                    .HasName("SpareId")
                    .IsUnique();

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
                    .WithOne(p => p.DidSpare)
                    .HasForeignKey<DidSpare>(d => d.SpareId)
                    .HasConstraintName("DidSpare_ibfk_2");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.DealerId)
                    .HasName("USER");

                entity.HasIndex(e => e.ModifierId)
                    .HasName("ModifierId");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.DealerId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.DeliveryAddress)
                    .IsRequired()
                    .HasColumnType("longtext");

				entity.Property(e => e.LastTimeModified)
					.HasColumnType("timestamp")
					.HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.ModifierId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("enum('Pending','Endorsed','Cancelled')");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("enum('Purchase','Reserve')");

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
					.HasDefaultValueSql("'CURRENT_TIMESTAMP'");

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

            modelBuilder.Entity<PriceHistory>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.Timestamp })
                    .HasName("PRIMARY");

                entity.Property(e => e.ProductId).HasColumnType("varchar(255)");

				entity.Property(e => e.Timestamp)
					.HasColumnType("timestamp")
					.HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PriceHistory)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("PriceHistory_ibfk_1");
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

                entity.Property(e => e.IconUrl).HasColumnType("varchar(2083)");

				entity.Property(e => e.LastTimeModified)
					.HasColumnType("timestamp")
					.HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.NormalizedName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.ReorderLevel).HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("enum('Available','Disabled')");
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
                    .HasColumnType("double")
                    .HasDefaultValueSql("'0'");

				entity.Property(e => e.StartTime)
					.HasColumnType("timestamp")
					.HasDefaultValueSql("'CURRENT_TIMESTAMP'");

				entity.Property(e => e.EndTime)
					.HasColumnType("timestamp")
					.HasDefaultValueSql("'CURRENT_TIMESTAMP'");

				entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasColumnType("varchar(2083)");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("enum('Available','Disabled')");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Promotion)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("ProductId_FK2");
            });

            modelBuilder.Entity<Spare>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("CATEGORYID");

                entity.Property(e => e.Id).HasColumnType("varchar(255)");

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Spare)
                    .HasForeignKey(d => d.CategoryId)
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

                entity.Property(e => e.IconUrl).HasColumnType("varchar(2083)");

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

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnType("enum('AreaManager','SalesOrderOfficer','Storemen','Administrator','Dealer','DispatchClerk','SalesManager')");

                entity.Property(e => e.SecurityStamp)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(256)");
            });
        }
    }
}
