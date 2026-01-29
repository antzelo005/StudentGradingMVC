using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradingMVC.Models;
using StudentGradingMVC.Models.ViewModels;

namespace StudentGradingMVC.Controllers
{
    [RoleGuard("Secretary")]
    public class SecretaryController : Controller
    {
        private readonly StudentGradingContext _context;

        public SecretaryController(StudentGradingContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        // =========================
        // Courses (List + Create)
        // =========================
        [HttpGet]
        public IActionResult Courses()
        {
            var courses = _context.Courses.AsNoTracking().OrderBy(c => c.Semester).ThenBy(c => c.Title).ToList();
            var professors = _context.Professors.AsNoTracking().OrderBy(p => p.FullName).ToList();

            ViewBag.Professors = professors;
            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCourse(string title, int semester, int professorId)
        {
            title = (title ?? "").Trim();
            if (string.IsNullOrWhiteSpace(title) || semester <= 0)
            {
                TempData["Err"] = "Συμπλήρωσε σωστά τίτλο/εξάμηνο.";
                return RedirectToAction("Courses");
            }

            var profExists = _context.Professors.Any(p => p.ProfessorId == professorId);
            if (!profExists)
            {
                TempData["Err"] = "Δεν βρέθηκε καθηγητής.";
                return RedirectToAction("Courses");
            }

            _context.Courses.Add(new Course
            {
                Title = title,
                Semester = semester,
                ProfessorId = professorId
            });

            _context.SaveChanges();
            TempData["Msg"] = "Το μάθημα δημιουργήθηκε.";
            return RedirectToAction("Courses");
        }

        // =========================
        // Assign professor to course
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignProfessor(int courseId, int professorId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                TempData["Err"] = "Δεν βρέθηκε μάθημα.";
                return RedirectToAction("Courses");
            }

            var profExists = _context.Professors.Any(p => p.ProfessorId == professorId);
            if (!profExists)
            {
                TempData["Err"] = "Δεν βρέθηκε καθηγητής.";
                return RedirectToAction("Courses");
            }

            course.ProfessorId = professorId;
            _context.SaveChanges();

            TempData["Msg"] = "Έγινε ανάθεση καθηγητή.";
            return RedirectToAction("Courses");
        }

        // =========================
        // Enrollments page
        // =========================
        [HttpGet]
        [RoleGuard("Secretary")]
        [HttpGet]
        public IActionResult Enrollments()
        {
            var rows = _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new SecretaryEnrollmentRowVM
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentFullName = (e.Student.FirstName + " " + e.Student.LastName),
                    CourseTitle = e.Course.Title,
                    Semester = e.Course.Semester
                })
                .OrderBy(x => x.Semester)
                .ThenBy(x => x.CourseTitle)
                .ThenBy(x => x.StudentFullName)
                .ToList();

            ViewBag.Students = _context.Students.AsNoTracking()
                .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                .ToList();

            ViewBag.Courses = _context.Courses.AsNoTracking()
                .OrderBy(c => c.Semester).ThenBy(c => c.Title)
                .ToList();

            return View(rows);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEnrollment(int studentId, int courseId)
        {
            var exists = _context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
            if (exists)
            {
                TempData["Err"] = "Ο φοιτητής είναι ήδη δηλωμένος σε αυτό το μάθημα.";
                return RedirectToAction("Enrollments");
            }

            _context.Enrollments.Add(new Enrollment { StudentId = studentId, CourseId = courseId });
            _context.SaveChanges();

            TempData["Msg"] = "Η δήλωση μαθήματος καταχωρήθηκε.";
            return RedirectToAction("Enrollments");
        }

        [RoleGuard("Secretary")]
        public IActionResult Assignments()
        {
            var rows = _context.Courses
                .AsNoTracking()
                .Include(c => c.Professor)
                .Select(c => new SecretaryAssignRowVM
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.Title,
                    Semester = c.Semester,
                    ProfessorFullName = c.Professor != null ? c.Professor.FullName : "-"
                })
                .OrderBy(x => x.Semester)
                .ThenBy(x => x.CourseTitle)
                .ToList();

            return View("Assignments", rows);

        }




    }
}
