using StudentGradingMVC.Models;

namespace StudentGradingMVC.Data
{
    public static class DbSeeder
    {
        public static void Seed(StudentGradingContext db)
        {
            // Αν έχει ήδη δεδομένα, μην ξανασπείρεις
            if (db.Users.Any() || db.Students.Any() || db.Professors.Any() || db.Courses.Any())
                return;

            // Users (για login)
            db.Users.AddRange(
                new User { Username = "stud1", Password = "1234", Role = "Student" },
                new User { Username = "stud2", Password = "1234", Role = "Student" },
                new User { Username = "prof1", Password = "1234", Role = "Professor" },
                new User { Username = "sec1", Password = "1234", Role = "Secretary" }
            );

            // Professors
            var prof = new Professor { Username = "prof1", FullName = "Dr. Nikos Papas" };
            db.Professors.Add(prof);

            // Students
            var s1 = new Student
            {
                FirstName = "Giorgos",
                LastName = "Kotsis",
                Username = "stud1"
            };

            var s2 = new Student
            {
                FirstName = "Maria",
                LastName = "Dima",
                Username = "stud2"
            };

            db.Students.AddRange(s1, s2);

            db.SaveChanges(); // για να πάρουν IDs

            // Courses
            var c1 = new Course { Title = "Data Bases", Semester = 3, ProfessorId = prof.ProfessorId };
            var c2 = new Course { Title = "Data Bases", Semester = 2, ProfessorId = prof.ProfessorId };
            db.Courses.AddRange(c1, c2);

            db.SaveChanges();

            // Enrollments (δηλώσεις)
            db.Enrollments.AddRange(
                new Enrollment { StudentId = s1.StudentId, CourseId = c1.CourseId },
                new Enrollment { StudentId = s2.StudentId, CourseId = c1.CourseId },
                new Enrollment { StudentId = s1.StudentId, CourseId = c2.CourseId }
            );

            db.SaveChanges();

            // Προαιρετικά: 1-2 έτοιμοι βαθμοί
            db.Grades.AddRange(
                new Grade { StudentId = s1.StudentId, CourseId = c1.CourseId, Value = 8.5m }
            );

            db.SaveChanges();
        }
    }
}
