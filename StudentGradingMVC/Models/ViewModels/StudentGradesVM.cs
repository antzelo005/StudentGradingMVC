namespace StudentGradingMVC.Models.ViewModels
{
    public class StudentCourseGradeVM
    {
        public int Semester { get; set; }
        public string Title { get; set; } = "";
        public decimal? Grade { get; set; }
    }

    public class StudentSemesterGradesVM
    {
        public int Semester { get; set; }
        public List<StudentCourseGradeVM> Items { get; set; } = new();
    }
}
