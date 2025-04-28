using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;

namespace WebApplication2.DbContexts
{
    public partial class TestdbContext : DbContext
    {
        public TestdbContext()
        {
        }

        public TestdbContext(DbContextOptions<TestdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;database=testdb6;user=root;password=123456789",
                    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            // Car
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.CarId).HasName("PRIMARY");
                entity.ToTable("cars");
                entity.Property(e => e.CarId).HasColumnName("car_id");
                entity.Property(e => e.Brand).HasMaxLength(50).HasColumnName("brand");
                entity.Property(e => e.Color).HasMaxLength(30).HasColumnName("color");
                entity.Property(e => e.HourlyRate).HasPrecision(10, 2).HasColumnName("daily_rate");
                entity.Property(e => e.IsAvailable).HasDefaultValueSql("'1'").HasColumnName("is_available");
                entity.Property(e => e.LicensePlate).HasMaxLength(20).HasColumnName("license_plate");
                entity.Property(e => e.Model).HasMaxLength(50).HasColumnName("model");
                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasMany(d => d.Drivers)
                    .WithMany(p => p.Cars)
                    .UsingEntity<Dictionary<string, object>>(
                        "CarDriver",
                        r => r.HasOne<Driver>().WithMany()
                            .HasForeignKey("DriverId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("car_driver_ibfk_2"),
                        l => l.HasOne<Car>().WithMany()
                            .HasForeignKey("CarId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("car_driver_ibfk_1"),
                        j =>
                        {
                            j.HasKey("CarId", "DriverId")
                                .HasName("PRIMARY")
                                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                            j.ToTable("car_driver");
                            j.HasIndex(new[] { "DriverId" }, "driver_id");
                            j.IndexerProperty<int>("CarId").HasColumnName("car_id");
                            j.IndexerProperty<int>("DriverId").HasColumnName("driver_id");
                        });
            });

            modelBuilder.Entity<Car>().HasData(
                new Car
                {
                    CarId = 1,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    Color = "Black",
                    LicensePlate = "ABC123",
                    HourlyRate = 25.50m,
                    IsAvailable = true
                },
                new Car
                {
                    CarId = 2,
                    Brand = "Honda",
                    Model = "Accord",
                    Year = 2019,
                    Color = "White",
                    LicensePlate = "XYZ789",
                    HourlyRate = 23.75m,
                    IsAvailable = true
                },
                new Car
                {
                    CarId = 3,
                    Brand = "Ford",
                    Model = "Focus",
                    Year = 2021,
                    Color = "Blue",
                    LicensePlate = "DEF456",
                    HourlyRate = 20.00m,
                    IsAvailable = true
                }
            );

            // Driver
            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.DriverId).HasName("PRIMARY");
                entity.ToTable("drivers");
                entity.Property(e => e.DriverId).HasColumnName("driver_id");
                entity.Property(e => e.FirstName).HasMaxLength(50).HasColumnName("first_name");
                entity.Property(e => e.HireDate).HasColumnName("hire_date");
                entity.Property(e => e.IsAvailable).HasDefaultValueSql("'1'").HasColumnName("is_available");
                entity.Property(e => e.LastName).HasMaxLength(50).HasColumnName("last_name");
                entity.Property(e => e.LicenseNumber).HasMaxLength(20).HasColumnName("license_number");
                entity.Property(e => e.Phone).HasMaxLength(15).HasColumnName("phone");
            });

            modelBuilder.Entity<Driver>().HasData(
                new Driver
                {
                    DriverId = 1,
                    FirstName = "Иван",
                    LastName = "Иванов",
                    LicenseNumber = "DL12345",
                    Phone = "+79001234567",
                    HireDate = DateOnly.FromDateTime(new DateTime(2020, 1, 15)),
                    IsAvailable = true
                },
                new Driver
                {
                    DriverId = 2,
                    FirstName = "Петр",
                    LastName = "Петров",
                    LicenseNumber = "DL67890",
                    Phone = "+79007654321",
                    HireDate = DateOnly.FromDateTime(new DateTime(2019, 5, 20)),
                    IsAvailable = true
                },
                new Driver
                {
                    DriverId = 3,
                    FirstName = "Сергей",
                    LastName = "Сергеев",
                    LicenseNumber = "DL54321",
                    Phone = "+79005556677",
                    HireDate = DateOnly.FromDateTime(new DateTime(2021, 3, 10)),
                    IsAvailable = true
                }
            );

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PRIMARY");
                entity.ToTable("orders");
                entity.HasIndex(e => e.CarId, "car_id");
                entity.HasIndex(e => e.DriverId, "driver_id");
                entity.HasIndex(e => e.UserId, "user_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.CarId).HasColumnName("car_id");
                entity.Property(e => e.DriverId).HasColumnName("driver_id");
                entity.Property(e => e.EndDate).HasColumnType("datetime").HasColumnName("end_date");
                entity.Property(e => e.StartDate).HasColumnType("datetime").HasColumnName("start_date");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'активен'").HasColumnName("status");
                entity.Property(e => e.TotalCost).HasPrecision(10, 2).HasColumnName("total_cost");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("orders_ibfk_2");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DriverId)
                    .HasConstraintName("orders_ibfk_3");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("orders_ibfk_1");
            });

            // Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("PRIMARY");
                entity.ToTable("payments");
                entity.HasIndex(e => e.OrderId, "order_id");
                entity.Property(e => e.PaymentId).HasColumnName("payment_id");
                entity.Property(e => e.Amount).HasPrecision(10, 2).HasColumnName("amount");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnType("datetime").HasColumnName("payment_date");
                entity.Property(e => e.PaymentMethod).HasMaxLength(50).HasColumnName("payment_method");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("payments_ibfk_1");
            });

            // Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId).HasName("PRIMARY");
                entity.ToTable("reviews");
                entity.HasIndex(e => e.OrderId, "order_id");
                entity.Property(e => e.ReviewId).HasColumnName("review_id");
                entity.Property(e => e.Comment).HasColumnType("text").HasColumnName("comment");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.ReviewDate).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnType("datetime").HasColumnName("review_date");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("reviews_ibfk_1");
            });

            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PRIMARY");
                entity.ToTable("roles");
                entity.HasIndex(e => e.RoleName, "role_name").IsUnique();
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.Description).HasColumnType("text").HasColumnName("description");
                entity.Property(e => e.RoleName).HasMaxLength(50).HasColumnName("role_name");

                entity.HasData(
                    new Role(1, "Гость", null),
                    new Role(2, "Водитель", null),
                    new Role(3, "Admin", null));
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PRIMARY");
                entity.ToTable("users");
                entity.HasIndex(e => e.RoleId, "role_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnType("datetime").HasColumnName("created_at");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
                entity.Property(e => e.FirstName).HasMaxLength(50).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasMaxLength(50).HasColumnName("last_name");
                entity.Property(e => e.Password).HasMaxLength(255).HasColumnName("password");
                entity.Property(e => e.Phone).HasMaxLength(15).HasColumnName("phone");
                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("users_ibfk_1");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.SessionId);

                entity.HasOne(s => s.User)
                    .WithMany(u => u.Sessions)
                    .HasForeignKey(d => d.UserId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}