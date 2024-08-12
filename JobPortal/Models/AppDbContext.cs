using Microsoft.EntityFrameworkCore;

namespace JobPortal.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppliedJobs> AppliedJobs { get; set; }
        public DbSet<Candidates> Candidates { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Employers> Employers { get; set; }
        public DbSet<JobCaterogies> JobCategories { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=MSI;Database =JOBPORTAL; Trusted_Connection = True; TrustServerCertificate = True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppliedJobs>()
                .HasOne(u => u.Jobs)
                .WithMany(i => i.ApplyJobs)
                .HasForeignKey(cp => cp.JobId);
            modelBuilder.Entity<AppliedJobs>()
                .HasOne(u => u.User)
                .WithMany(i => i.AppliedJobs)
                .HasForeignKey(cp => cp.UserId);
            modelBuilder.Entity<Candidates>()
                .HasOne(u => u.Country)
                .WithMany(i => i.Candidates)
                .HasForeignKey(cp => cp.CountryId);
            modelBuilder.Entity<Candidates>()
                .HasOne(u => u.User)
                .WithOne(i => i.Candidates)
                .HasForeignKey<Candidates>(cp => cp.UserId);
            modelBuilder.Entity<Employers>()
                .HasOne(u => u.User)
                .WithOne(i => i.Employers)
                .HasForeignKey<Employers>(cp => cp.UserId);
            modelBuilder.Entity<Employers>()
                .HasOne(u => u.Country)
                .WithMany(i => i.Employers)
                .HasForeignKey(cp => cp.CountryId);
            modelBuilder.Entity<Jobs>()
                .HasOne(u => u.Country)
                .WithMany(i => i.Jobs)
                .HasForeignKey(cp => cp.CountryId);
            modelBuilder.Entity<Jobs>()
                .HasOne(u => u.JobCaterogies)
                .WithMany(i => i.Jobs)
                .HasForeignKey(cp => cp.JobCategoryId);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(i => i.Users)
                .HasForeignKey(cp => cp.RoleId);
            modelBuilder.Entity<Jobs>()
                .HasOne(u => u.Employers)
                .WithMany(u => u.Jobs)
                .HasForeignKey(cp => cp.EmployerId);
        }
    }
}
