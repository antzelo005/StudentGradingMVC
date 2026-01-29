using System.ComponentModel.DataAnnotations;

namespace StudentGradingMVC.Models.ViewModels
{
    public class ProfessorCourseOptionVM
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = "";
        public int Semester { get; set; }
    }

    public class GradeRowVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";

        [Range(0, 10, ErrorMessage = "The grade should be 0–10")]
        public decimal? Grade { get; set; }
    }

    public class ProfessorGradeEntryVM
    {
        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = "";
        public int CourseSemester { get; set; }

        public List<GradeRowVM> Rows { get; set; } = new();
    }

    public class ProfessorGradeEntrySelectCourseVM
    {
        public List<ProfessorCourseOptionVM> Courses { get; set; } = new();
    }
}
