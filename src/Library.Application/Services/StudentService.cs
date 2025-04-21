using AutoMapper;
using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Domain.Validators;
using Library.Application.Contracts.Services;
using Library.Application.DTOs.Pagination;
using Library.Application.DTOs.Student;
using Library.Application.Notifications;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Services;

public class StudentService : BaseService, IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IPasswordHasher<Student> _passwordHasher;

    public StudentService(INotificator notificator, IMapper mapper, IStudentRepository studentRepository,
        IPasswordHasher<Student> passwordHasher) : base(notificator, mapper)
    {
        _studentRepository = studentRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<StudentDto?> Add(AddStudentDto dto)
    {
        if (!await ValidationsToAdd(dto))
            return null;

        var student = Mapper.Map<Student>(dto);
        student.Password = _passwordHasher.HashPassword(student, dto.Password);
        student.NumberOfLoansAllowed = 5;
        student.NumberOfLoansTaken = 0;
        student.Blocked = false;
        student.Active = true;
        _studentRepository.Add(student);

        return await CommitChanges() ? Mapper.Map<StudentDto>(student) : null;
    }

    public async Task<StudentDto?> Update(int id, UpdateStudentDto dto)
    {
        if (!await ValidationsToUpdate(id, dto))
            return null;

        var student = await _studentRepository.FirstOrDefault(s => s.Id == id);
        student!.Name = dto.Name;
        student.Registration = dto.Registration;
        student.Course = dto.Course;
        student.Email = dto.Email;
        student.Password = dto.Password;
        student.Password = _passwordHasher.HashPassword(student, dto.Password);
        _studentRepository.Update(student);

        return await CommitChanges() ? Mapper.Map<StudentDto>(student) : null;
    }

    public async Task<PaginationDto<StudentDto>> Search(SearchStudentDto dto)
    {
        var result = await _studentRepository.Search(dto.Id, dto.Name, dto.Email, dto.Registration,
            dto.Course, dto.Active, dto.NumberOfItemsPerPage, dto.CurrentPage);

        return new PaginationDto<StudentDto>
        {
            TotalItems = result.TotalItems,
            NumberOfItemsPerPage = result.NumberOfItemsPerPage,
            NumberOfPages = result.NumberOfPages,
            CurrentPage = result.CurrentPage,
            Items = Mapper.Map<List<StudentDto>>(result.Items)
        };
    }

    public async Task<List<StudentDto>> GetAll()
    {
        var students = await _studentRepository.GetAll();
        return Mapper.Map<List<StudentDto>>(students);
    }

    public async Task Active(int id)
    {
        var student = await _studentRepository.FirstOrDefault(s => s.Id == id);
        if (student == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (student.Active)
        {
            Notificator.Handle("The informed student is already active");
            return;
        }

        student.Active = true;
        _studentRepository.Update(student);

        await CommitChanges();
    }

    public async Task Inactive(int id)
    {
        var student = await _studentRepository.FirstOrDefault(s => s.Id == id);
        if (student == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (student.Active == false)
        {
            Notificator.Handle("The informed student is already inactive");
            return;
        }

        if (student.NumberOfLoansTaken > 0)
        {
            Notificator.Handle("The student cannot be deactivated because he/she has borrowed books");
            return;
        }

        student.Active = false;
        _studentRepository.Update(student);

        await CommitChanges();
    }

    private async Task<bool> ValidationsToAdd(AddStudentDto dto)
    {
        var student = Mapper.Map<Student>(dto);
        var validator = new StudentValidator();

        var result = await validator.ValidateAsync(student);
        if (!result.IsValid)
        {
            Notificator.Handle(result.Errors);
            return false;
        }

        var studentWithSameRegistration = await _studentRepository.FirstOrDefault(s =>
            s.Registration == dto.Registration);
        if (studentWithSameRegistration != null)
        {
            Notificator.Handle("There is already a student registered with the registration number provided");
            return false;
        }

        var studentWithSameEmail = await _studentRepository.FirstOrDefault(s => s.Email == dto.Email);
        if (studentWithSameEmail != null)
        {
            Notificator.Handle("There is already a student registered with the email address provided");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidationsToUpdate(int id, UpdateStudentDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("The ID provided in the URL must be the same as the ID provided in the JSON");
            return false;
        }

        var studentExist = await _studentRepository.FirstOrDefault(s => s.Id == id);
        if (studentExist == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (studentExist.Active == false)
        {
            Notificator.Handle("You cannot update a student who is inactive");
            return false;
        }

        var student = Mapper.Map<Student>(dto);
        var validator = new StudentValidator();

        var result = await validator.ValidateAsync(student);
        if (!result.IsValid)
        {
            Notificator.Handle(result.Errors);
            return false;
        }

        return true;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _studentRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("An error occurred while saving changes");
        return false;
    }
}