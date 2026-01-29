using Microsoft.EntityFrameworkCore;

namespace StudentGradingMVC.Models
{
    public class StudentGradingContext : DbContext
    {
        public StudentGradingContext(DbContextOptions<StudentGradingContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Professor> Professors => Set<Professor>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Grade> Grades => Set<Grade>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grade>()
                .Property(g => g.Value)
                .HasPrecision(4, 2); 

            base.OnModelCreating(modelBuilder);
        }

    }
}
