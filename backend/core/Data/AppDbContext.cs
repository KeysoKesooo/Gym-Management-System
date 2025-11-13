using Microsoft.EntityFrameworkCore;
using GymManagement.Core.Models.UserModel;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.Models.WalkinModel;

namespace GymManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<WalkinGuest> WalkinGuests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.User)
                .WithMany(u => u.Attendances)
                .HasForeignKey(a => a.UserId)
                .IsRequired(); // ✅ Keeps FK required at DB level
        }

    }
}
