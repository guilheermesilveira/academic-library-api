namespace Library.Application.DTOs.Student;

public class StudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Registration { get; set; } = null!;
    public string Course { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int NumberOfLoansAllowed { get; set; }
    public int NumberOfLoansTaken { get; set; }
    public bool Blocked { get; set; }
    public bool Active { get; set; }
}