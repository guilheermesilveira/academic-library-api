namespace Library.Application.DTOs.Student;

public class UpdateStudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Registration { get; set; } = null!;
    public string Course { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}