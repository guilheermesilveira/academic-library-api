using AutoMapper;
using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Application.Contracts.Services;
using Library.Application.DTOs.Loan;
using Library.Application.DTOs.Pagination;
using Library.Application.Notifications;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Services;

public class LoanService : BaseService, ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IPasswordHasher<Student> _passwordHasher;

    public LoanService(INotificator notificator, IMapper mapper, ILoanRepository loanRepository,
        IStudentRepository studentRepository, IBookRepository bookRepository,
        IPasswordHasher<Student> passwordHasher) : base(notificator, mapper)
    {
        _loanRepository = loanRepository;
        _studentRepository = studentRepository;
        _bookRepository = bookRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoanDto?> Lend(LendDto dto)
    {
        if (!await ValidationsToLend(dto))
            return null;

        var student = await _studentRepository.FirstOrDefault(s => s.Registration == dto.StudentRegistration);
        var book = await _bookRepository.FirstOrDefault(b => b.Code == dto.BookCode);

        var result = _passwordHasher.VerifyHashedPassword(student!, student!.Password, dto.StudentPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Incorrect password");
            return null;
        }

        student.NumberOfLoansTaken += 1;
        _studentRepository.Update(student);

        book!.QuantityOfCopiesAvailableForLoan -= 1;
        if (book.QuantityOfCopiesAvailableForLoan == 0)
        {
            book.BookStatus = EBookStatus.Unavailable;
        }

        _bookRepository.Update(book);

        var loan = new Loan();
        loan.LoanDate = DateTime.Today;
        loan.ExpectedRepaymentDate = DateTime.Today.AddDays(10);
        loan.LoanStatus = ELoanStatus.Loaned;
        loan.NumberOfRenewalsAllowed = 5;
        loan.NumberOfRenewalsCompleted = 0;
        loan.StudentId = student.Id;
        loan.BookId = book.Id;
        loan.Active = true;
        _loanRepository.Add(loan);

        return await CommitChanges() ? Mapper.Map<LoanDto>(loan) : null;
    }

    public async Task<LoanDto?> Renew(int id, RenewDto dto)
    {
        if (!await ValidationsToRenew(id, dto))
            return null;

        var loan = await _loanRepository.GetById(id);
        var student = loan!.Student;

        var result = _passwordHasher.VerifyHashedPassword(student, student.Password, dto.StudentPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Incorrect password");
            return null;
        }

        loan.LoanDate = DateTime.Today;
        loan.ExpectedRepaymentDate = DateTime.Today.AddDays(10);
        loan.LoanStatus = ELoanStatus.Renewed;
        loan.NumberOfRenewalsCompleted += 1;
        _loanRepository.Update(loan);

        return await CommitChanges() ? Mapper.Map<LoanDto>(loan) : null;
    }

    public async Task<LoanDto?> Deliver(int id, DeliverDto dto)
    {
        if (!await ValidationsToDeliver(id, dto))
            return null;

        var loan = await _loanRepository.GetById(id);
        var student = loan!.Student;
        var book = loan.Book;

        student.NumberOfLoansTaken -= 1;
        _studentRepository.Update(student);

        if (book.BookStatus == EBookStatus.Unavailable)
        {
            book.BookStatus = EBookStatus.Available;
        }

        book.QuantityOfCopiesAvailableForLoan += 1;
        _bookRepository.Update(book);

        loan.Active = false;
        loan.RepaymentDateCompleted = DateTime.Today;
        loan.LoanStatus = loan.RepaymentDateCompleted > loan.ExpectedRepaymentDate
            ? ELoanStatus.DeliveredLate
            : ELoanStatus.Delivered;
        _loanRepository.Update(loan);

        if (!await CommitChanges())
            return null;

        if (student.Blocked)
        {
            var lateLoan = await _loanRepository.FirstOrDefault(l =>
                l.StudentId == student.Id &&
                DateTime.Today > l.ExpectedRepaymentDate &&
                (l.LoanStatus == ELoanStatus.Loaned || l.LoanStatus == ELoanStatus.Renewed));
            if (lateLoan == null)
            {
                student.Blocked = false;
                _studentRepository.Update(student);
                await _studentRepository.UnitOfWork.Commit();
            }
        }

        return Mapper.Map<LoanDto>(loan);
    }

    public async Task<PaginationDto<LoanDto>> Search(SearchLoanDto dto)
    {
        var result = await _loanRepository.Search(dto.Id, dto.StudentId,
            dto.StudentRegistration, dto.BookId, dto.BookCode, dto.Active, dto.NumberOfItemsPerPage,
            dto.CurrentPage);

        return new PaginationDto<LoanDto>
        {
            TotalItems = result.TotalItems,
            NumberOfItemsPerPage = result.NumberOfItemsPerPage,
            NumberOfPages = result.NumberOfPages,
            CurrentPage = result.CurrentPage,
            Items = Mapper.Map<List<LoanDto>>(result.Items)
        };
    }

    public async Task<List<LoanDto>> GetAll()
    {
        var loans = await _loanRepository.GetAll();
        return Mapper.Map<List<LoanDto>>(loans);
    }

    public async Task Active(int id)
    {
        var loan = await _loanRepository.FirstOrDefault(l => l.Id == id);
        if (loan == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (loan.Active)
        {
            Notificator.Handle("The informed loan is already active");
            return;
        }

        loan.Active = true;
        _loanRepository.Update(loan);

        await CommitChanges();
    }

    public async Task Inactive(int id)
    {
        var loan = await _loanRepository.FirstOrDefault(l => l.Id == id);
        if (loan == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (loan.Active == false)
        {
            Notificator.Handle("The informed loan is already inactive");
            return;
        }

        if (loan.LoanStatus != ELoanStatus.Delivered && loan.LoanStatus != ELoanStatus.DeliveredLate)
        {
            Notificator.Handle("The loan cannot be deactivated as it has not yet been delivered");
            return;
        }

        loan.Active = false;
        _loanRepository.Update(loan);

        await CommitChanges();
    }

    private async Task<bool> ValidationsToLend(LendDto dto)
    {
        var student = await _studentRepository.FirstOrDefault(s => s.Registration == dto.StudentRegistration);
        if (student == null)
        {
            Notificator.Handle("Student not found with the informed registration");
            return false;
        }

        if (student.Active == false)
        {
            Notificator.Handle("It is not possible to make a loan to an inactive student");
            return false;
        }

        var book = await _bookRepository.FirstOrDefault(b => b.Code == dto.BookCode);
        if (book == null)
        {
            Notificator.Handle("Book not found with the code provided");
            return false;
        }

        if (book.Active == false)
        {
            Notificator.Handle("You cannot borrow an inactive book");
            return false;
        }

        if (student.NumberOfLoansTaken == student.NumberOfLoansAllowed)
        {
            Notificator.Handle("The student has already reached the loan limit");
            return false;
        }

        if (book.BookStatus == EBookStatus.Unavailable)
        {
            Notificator.Handle("There are currently no copies of this book available");
            return false;
        }

        if (student.Blocked)
        {
            Notificator.Handle("The student is blocked");
            return false;
        }

        var loanLate = await _loanRepository.FirstOrDefault(l =>
            l.StudentId == student.Id &&
            DateTime.Today > l.ExpectedRepaymentDate &&
            (l.LoanStatus == ELoanStatus.Loaned || l.LoanStatus == ELoanStatus.Renewed));
        if (loanLate != null)
        {
            student.Blocked = true;
            _studentRepository.Update(student);
            await _studentRepository.UnitOfWork.Commit();

            Notificator.Handle("The student has a book that has not been delivered, so he/she will be blocked");
            return false;
        }

        var loanExist = await _loanRepository.FirstOrDefault(l =>
            l.BookId == book.Id &&
            l.StudentId == student.Id &&
            (l.LoanStatus == ELoanStatus.Loaned || l.LoanStatus == ELoanStatus.Renewed));
        if (loanExist != null)
        {
            Notificator.Handle("The student already has a borrowed or renewed copy of the same book");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidationsToRenew(int id, RenewDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("The ID provided in the URL must be the same as the ID provided in the JSON");
            return false;
        }

        var loan = await _loanRepository.GetById(id);
        if (loan == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (loan.LoanStatus is ELoanStatus.Delivered or ELoanStatus.DeliveredLate)
        {
            Notificator.Handle("The loan has already been delivered");
            return false;
        }

        if (loan.NumberOfRenewalsCompleted == loan.NumberOfRenewalsAllowed)
        {
            Notificator.Handle("The student has already reached the renewal limit for this book");
            return false;
        }

        var registeredStudent = await _studentRepository.FirstOrDefault(s =>
            s.Registration == loan.Student.Registration);
        var studentInformed = await _studentRepository.FirstOrDefault(s =>
            s.Registration == dto.StudentRegistration);

        if (studentInformed == null)
        {
            Notificator.Handle("Student not found with the informed registration");
            return false;
        }

        if (registeredStudent!.Registration != studentInformed.Registration)
        {
            Notificator.Handle("The registration number provided is not part of this loan");
            return false;
        }

        var bookRegistered = await _bookRepository.FirstOrDefault(b =>
            b.Code == loan.Book.Code);
        var bookInformed = await _bookRepository.FirstOrDefault(b =>
            b.Code == dto.BookCode);

        if (bookInformed == null)
        {
            Notificator.Handle("Book not found with the code provided");
            return false;
        }

        if (bookRegistered!.Code != bookInformed.Code)
        {
            Notificator.Handle("The book code provided is not part of this loan");
            return false;
        }

        if (studentInformed.Blocked)
        {
            Notificator.Handle("The student is blocked");
            return false;
        }

        var loanLate = await _loanRepository.FirstOrDefault(l =>
            l.StudentId == studentInformed.Id &&
            DateTime.Today > l.ExpectedRepaymentDate &&
            (l.LoanStatus == ELoanStatus.Loaned || l.LoanStatus == ELoanStatus.Renewed));
        if (loanLate != null)
        {
            studentInformed.Blocked = true;
            _studentRepository.Update(studentInformed);
            await _studentRepository.UnitOfWork.Commit();

            Notificator.Handle("The student has a book that has not been delivered, so he/she will be blocked");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidationsToDeliver(int id, DeliverDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("The ID provided in the URL must be the same as the ID provided in the JSON");
            return false;
        }

        var loan = await _loanRepository.GetById(id);
        if (loan == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (loan.LoanStatus is ELoanStatus.Delivered or ELoanStatus.DeliveredLate)
        {
            Notificator.Handle("The loan has already been delivered");
            return false;
        }

        var registeredStudent = await _studentRepository.FirstOrDefault(s =>
            s.Registration == loan.Student.Registration);
        var studentInformed = await _studentRepository.FirstOrDefault(s =>
            s.Registration == dto.StudentRegistration);

        if (studentInformed == null)
        {
            Notificator.Handle("Student not found with the informed registration");
            return false;
        }

        if (registeredStudent!.Registration != studentInformed.Registration)
        {
            Notificator.Handle("The registration number provided is not part of this loan");
            return false;
        }

        var bookRegistered = await _bookRepository.FirstOrDefault(b =>
            b.Code == loan.Book.Code);
        var bookInformed = await _bookRepository.FirstOrDefault(b =>
            b.Code == dto.BookCode);

        if (bookInformed == null)
        {
            Notificator.Handle("Book not found with the code provided");
            return false;
        }

        if (bookRegistered!.Code != bookInformed.Code)
        {
            Notificator.Handle("The book code provided is not part of this loan");
            return false;
        }

        return true;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _loanRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("An error occurred while saving changes");
        return false;
    }
}