namespace StudentGradingMVC.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = "";
        public int Semester { get; set; }

        public int ProfessorId { get; set; }
        public Professor Professor { get; set; } = null!;
    }
}
