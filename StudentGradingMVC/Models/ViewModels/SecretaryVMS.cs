namespace StudentGradingMVC.Models.ViewModels
{
    public class SecretaryEnrollmentRowVM
    {
        public int EnrollmentId { get; set; }
        public string StudentFullName { get; set; } = "";
        public string CourseTitle { get; set; } = "";
        public int Semester { get; set; }
    }

    public class SecretaryAssignRowVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public int Semester { get; set; }
        public string ProfessorFullName { get; set; } = "";
    }
}
