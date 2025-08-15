using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Models;


namespace CentralAddressSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Province> Provinces { get; set; } = null!;
        public DbSet<District> Districts { get; set; } = null!;
        public DbSet<LocalBody> LocalBodies { get; set; } = null!;
    
        public DbSet<Address> Addresses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map to existing table names
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Province>().ToTable("Provinces");
            modelBuilder.Entity<District>().ToTable("Districts");
            modelBuilder.Entity<LocalBody>().ToTable("LocalBodies");
            modelBuilder.Entity<Address>().ToTable("Addresses");
            modelBuilder.Entity<User>().ToTable("AspNetUsers");

            // Configure Address.UserID to reference AspNetUsers.Id
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserID)
                .HasConstraintName("FK_Addresses_AspNetUsers");


            // Add unique index for Email
            modelBuilder.Entity<User>()
             .HasIndex(u => u.Email)
                 .IsUnique();
        }

        }
        
    }
