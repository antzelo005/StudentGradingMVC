using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradingMVC.Models;
using StudentGradingMVC.Models.ViewModels;

namespace StudentGradingMVC.Controllers
{
    [RoleGuard("Student")]
    public class StudentController : Controller
    {
        private readonly StudentGradingContext _context;

        public StudentController(StudentGradingContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult GradesByCourse()
        {
            var username = Request.Cookies["Username"] ?? "";
            var student = _context.Students.AsNoTracking().FirstOrDefault(s => s.Username == username);
            if (student == null) return RedirectToAction("Login", "Account");

            var data = _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == student.StudentId)
                .Join(_context.Courses.AsNoTracking(),
                      e => e.CourseId,
                      c => c.CourseId,
                      (e, c) => new { c.CourseId, c.Title, c.Semester })
                .GroupJoin(_context.Grades.AsNoTracking().Where(g => g.StudentId == student.StudentId),
                           x => x.CourseId,
                           g => g.CourseId,
                           (x, gs) => new StudentCourseGradeVM
                           {
                               Title = x.Title,
                               Semester = x.Semester,
                               Grade = gs.Select(z => (decimal?)z.Value).FirstOrDefault()
                           })
                .OrderBy(x => x.Semester)
                .ThenBy(x => x.Title)
                .ToList();

            return View(data);
        }

        public IActionResult GradesBySemester()
        {
            var username = Request.Cookies["Username"] ?? "";
            var student = _context.Students.AsNoTracking().FirstOrDefault(s => s.Username == username);
            if (student == null) return RedirectToAction("Login", "Account");

            var flat = _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == student.StudentId)
                .Join(_context.Courses.AsNoTracking(),
                      e => e.CourseId,
                      c => c.CourseId,
                      (e, c) => new { c.CourseId, c.Title, c.Semester })
                .GroupJoin(_context.Grades.AsNoTracking().Where(g => g.StudentId == student.StudentId),
                           x => x.CourseId,
                           g => g.CourseId,
                           (x, gs) => new StudentCourseGradeVM
                           {
                               Title = x.Title,
                               Semester = x.Semester,
                               Grade = gs.Select(z => (decimal?)z.Value).FirstOrDefault()
                           })
                .ToList();

            var data = flat
                .GroupBy(x => x.Semester)
                .OrderBy(g => g.Key)
                .Select(g => new StudentSemesterVM
                {
                    Semester = g.Key,
                    Items = g.OrderBy(x => x.Title).ToList()
                })
                .ToList();

            return View(data);
        }

        public IActionResult OverallGrades()
        {
            var username = Request.Cookies["Username"] ?? "";
            var student = _context.Students.AsNoTracking().FirstOrDefault(s => s.Username == username);
            if (student == null) return RedirectToAction("Login", "Account");

            var graded = _context.Grades
                .AsNoTracking()
                .Where(g => g.StudentId == student.StudentId)
                .Select(g => g.Value)
                .ToList();

            var avg = graded.Count == 0 ? (decimal?)null : graded.Average();
            return View(avg);
        }
    }
}
