namespace StudentGradingMVC.Models.ViewModels
{
    public class StudentSemesterVM
    {
        public int Semester { get; set; }
        public List<StudentCourseGradeVM> Items { get; set; } = new();
    }
}
