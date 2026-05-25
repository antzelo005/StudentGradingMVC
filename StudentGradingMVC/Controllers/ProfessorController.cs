using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradingMVC.Models;
using StudentGradingMVC.Models.ViewModels;

namespace StudentGradingMVC.Controllers
{
    [RoleGuard("Professor")]
    public class ProfessorController : Controller
    {
        private readonly StudentGradingContext _context;

        public ProfessorController(StudentGradingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // STEP 1: Course selection
        // =========================
        [HttpGet]
        public IActionResult GradeEntry()
        {
            var username = Request.Cookies["Username"];

            // TODO: If ProfessorId exists directly on User, update this lookup accordingly.
            // Assumption: there is a Professors table and User.Username matches a Professor record.
            var professor = _context.Professors
                .AsNoTracking()
                .FirstOrDefault(p => p.Username == username);

            if (professor == null)
                return RedirectToAction("Login", "Account");

            var courses = _context.Courses
                .AsNoTracking()
                .Where(c => c.ProfessorId == professor.ProfessorId)
                .OrderBy(c => c.Semester)
                .ThenBy(c => c.Title)
                .Select(c => new ProfessorCourseOptionVM
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Semester = c.Semester
                })
                .ToList();

            var vm = new ProfessorGradeEntrySelectCourseVM
            {
                Courses = courses
            };

            return View(vm);
        }

        // =========================
        // STEP 2: Grading form
        // =========================
        [HttpGet]
        public IActionResult GradeEntryForm(int courseId)
        {
            var username = Request.Cookies["Username"];
            var professor = _context.Professors
                .AsNoTracking()
                .FirstOrDefault(p => p.Username == username);

            if (professor == null)
                return RedirectToAction("Login", "Account");

            var course = _context.Courses
                .AsNoTracking()
                .FirstOrDefault(c => c.CourseId == courseId);

            if (course == null)
                return NotFound();

            // Security: the professor must own this course
            if (course.ProfessorId != professor.ProfessorId)
                return RedirectToAction("GradeEntry");

            // Get the students enrolled in this course.
            // Assumption: Enrollments(StudentId, CourseId) and Students(StudentId, FirstName, LastName)
            var declared = _context.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .Select(e => new
                {
                    e.StudentId
                })
                .ToList();

            var studentIds = declared.Select(x => x.StudentId).ToList();

            var students = _context.Students
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.StudentId))
                .Select(s => new
                {
                    s.StudentId,
                    Name = (s.FirstName + " " + s.LastName)
                })
                .ToList();

            // Assumption: Grades(StudentId, CourseId, Value)
            var existingGrades = _context.Grades
                .AsNoTracking()
                .Where(g => g.CourseId == courseId && studentIds.Contains(g.StudentId))
                .ToDictionary(g => g.StudentId, g => g.Value);

            var vm = new ProfessorGradeEntryVM
            {
                CourseId = course.CourseId,
                CourseTitle = course.Title,
                CourseSemester = course.Semester,
                Rows = students
                    .OrderBy(s => s.Name)
                    .Select(s => new GradeRowVM
                    {
                        StudentId = s.StudentId,
                        StudentName = s.Name,
                        Grade = existingGrades.ContainsKey(s.StudentId) ? existingGrades[s.StudentId] : null
                    })
                    .ToList()
            };

            return View(vm);
        }
        [HttpGet]
        public IActionResult GradedCourses()
        {
            var username = Request.Cookies["Username"];
            var professor = _context.Professors
                .AsNoTracking()
                .FirstOrDefault(p => p.Username == username);

            if (professor == null)
                return RedirectToAction("Login", "Account");

            // Courses assigned to the professor.
            var rows = _context.Courses
                .AsNoTracking()
                .Where(c => c.ProfessorId == professor.ProfessorId)
                .OrderBy(c => c.Semester)
                .ThenBy(c => c.Title)
                .Select(c => new ProfessorCourseOptionVM
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Semester = c.Semester
                })
                .ToList();

            return View(rows);
        }

        // =========================
        // SAVE grades
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GradeEntryForm(ProfessorGradeEntryVM model)
        {
            // Server-side validation for the grade range and required fields.
            if (!ModelState.IsValid)
                return View(model);

            var username = Request.Cookies["Username"];
            var professor = _context.Professors
                .AsNoTracking()
                .FirstOrDefault(p => p.Username == username);

            if (professor == null)
                return RedirectToAction("Login", "Account");

            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == model.CourseId);

            if (course == null)
                return NotFound();

            if (course.ProfessorId != professor.ProfessorId)
                return RedirectToAction("GradeEntry");

            // Load existing grades for this course.
            var existing = _context.Grades
                .Where(g => g.CourseId == model.CourseId)
                .ToList();

            foreach (var row in model.Rows)
            {
                // Skip rows without a grade.
                if (row.Grade == null) continue;

                var grade = existing.FirstOrDefault(g => g.StudentId == row.StudentId);

                if (grade == null)
                {
                    _context.Grades.Add(new Grade
                    {
                        CourseId = model.CourseId,
                        StudentId = row.StudentId,
                        Value = row.Grade.Value
                    });
                }
                else
                {
                    grade.Value = row.Grade.Value;
                    _context.Grades.Update(grade);
                }
            }

            _context.SaveChanges();

            TempData["Msg"] = "Grades were saved successfully.";
            return RedirectToAction("GradeEntryForm", new { courseId = model.CourseId });
        }
    }
}
