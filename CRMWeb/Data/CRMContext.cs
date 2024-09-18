using Microsoft.EntityFrameworkCore;
using CRMWeb.Models;

namespace CRMWeb.Data
{
    public class CRMContext : DbContext
    {
        public CRMContext(DbContextOptions<CRMContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<AppointmentProduct> AppointmentProducts { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite primary keys for many-to-many relationships
            modelBuilder.Entity<AppointmentProduct>()
                .HasKey(ap => new { ap.AppointmentID, ap.ProductID });

            modelBuilder.Entity<UserProduct>()
                .HasKey(up => new { up.UserID, up.ProductID });

        }
    }
}
