namespace StudentGradingMVC.Models
{
    public class Grade
    {
        public int GradeId { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public decimal Value { get; set; }
    }
}
