using AutoMapper;
using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Validators;
using Library.Application.Configurations;
using Library.Application.Contracts.Services;
using Library.Application.DTOs.Book;
using Library.Application.DTOs.Pagination;
using Library.Application.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Library.Application.Services;

public class BookService : BaseService, IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly string _imagePath;

    public BookService(INotificator notificator, IMapper mapper, IBookRepository bookRepository,
        IOptions<StorageSettings> storageSettings) : base(notificator, mapper)
    {
        _bookRepository = bookRepository;
        _imagePath = storageSettings.Value.ImagePath;
    }

    public async Task<BookDto?> Add(AddBookDto dto)
    {
        if (!await ValidationsToAdd(dto))
            return null;

        var book = Mapper.Map<Book>(dto);
        book.Code = await GenerateCode();
        book.QuantityOfCopiesAvailableForLoan = book.QuantityOfCopiesAvailableInStock;
        book.BookStatus = EBookStatus.Available;
        book.Active = true;
        _bookRepository.Add(book);

        return await CommitChanges() ? Mapper.Map<BookDto>(book) : null;
    }

    public async Task<BookDto?> Update(int id, UpdateBookDto dto)
    {
        if (!await ValidationsToUpdate(id, dto))
            return null;

        var book = await _bookRepository.FirstOrDefault(b => b.Id == id);
        book!.Title = dto.Title;
        book.Author = dto.Author;
        book.Edition = dto.Edition;
        book.Publisher = dto.Publisher;
        book.Category = dto.Category;
        book.YearOfPublication = dto.YearOfPublication;
        book.QuantityOfCopiesAvailableInStock = dto.QuantityOfCopiesAvailableInStock;
        book.QuantityOfCopiesAvailableForLoan = book.QuantityOfCopiesAvailableInStock;
        _bookRepository.Update(book);

        return await CommitChanges() ? Mapper.Map<BookDto>(book) : null;
    }

    public async Task<BookDto?> UploadCover(int id, ICollection<IFormFile>? files)
    {
        var book = await _bookRepository.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        if (files == null || files.Count == 0)
        {
            Notificator.Handle("No files loaded");
            return null;
        }

        foreach (var file in files)
        {
            if (!IsImage(file))
            {
                Notificator.Handle("Only image files are allowed");
                return null;
            }

            if (!string.IsNullOrEmpty(book.BookCover))
            {
                var previousPath = Path.Combine(_imagePath, book.BookCover);
                if (File.Exists(previousPath))
                    File.Delete(previousPath);
            }

            var fileName = DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(_imagePath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            book.BookCover = fileName;
            _bookRepository.Update(book);
        }

        return await CommitChanges() ? Mapper.Map<BookDto>(book) : null;
    }

    public async Task<PaginationDto<BookDto>> Search(SearchBookDto dto)
    {
        var result = await _bookRepository.Search(dto.Id, dto.Title, dto.Author,
            dto.Publisher, dto.Category, dto.Code, dto.Active, dto.NumberOfItemsPerPage, dto.CurrentPage);

        return new PaginationDto<BookDto>
        {
            TotalItems = result.TotalItems,
            NumberOfItemsPerPage = result.NumberOfItemsPerPage,
            NumberOfPages = result.NumberOfPages,
            CurrentPage = result.CurrentPage,
            Items = Mapper.Map<List<BookDto>>(result.Items)
        };
    }

    public async Task<List<BookDto>> GetAll()
    {
        var books = await _bookRepository.GetAll();
        return Mapper.Map<List<BookDto>>(books);
    }

    public async Task Active(int id)
    {
        var book = await _bookRepository.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (book.Active)
        {
            Notificator.Handle("The informed book is already active");
            return;
        }

        book.Active = true;
        _bookRepository.Update(book);

        await CommitChanges();
    }

    public async Task Inactive(int id)
    {
        var book = await _bookRepository.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (book.Active == false)
        {
            Notificator.Handle("The informed book is already inactive");
            return;
        }

        if (book.QuantityOfCopiesAvailableForLoan != book.QuantityOfCopiesAvailableInStock)
        {
            Notificator.Handle("The book cannot be deactivated, as it has copies on loan");
            return;
        }

        book.Active = false;
        _bookRepository.Update(book);

        await CommitChanges();
    }

    private async Task<bool> ValidationsToAdd(AddBookDto dto)
    {
        var book = Mapper.Map<Book>(dto);
        var validator = new BookValidator();

        var result = await validator.ValidateAsync(book);
        if (!result.IsValid)
        {
            Notificator.Handle(result.Errors);
            return false;
        }

        var bookExist = await _bookRepository.FirstOrDefault(b =>
            b.Title == dto.Title &&
            b.Author == dto.Author &&
            b.Edition == dto.Edition &&
            b.Publisher == dto.Publisher);
        if (bookExist != null)
        {
            Notificator.Handle("There is already a registered book with this information");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidationsToUpdate(int id, UpdateBookDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("The ID provided in the URL must be the same as the ID provided in the JSON");
            return false;
        }

        var bookExist = await _bookRepository.FirstOrDefault(b => b.Id == id);
        if (bookExist == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (bookExist.Active == false)
        {
            Notificator.Handle("You cannot update a book that is inactive");
            return false;
        }

        if (bookExist.QuantityOfCopiesAvailableForLoan < bookExist.QuantityOfCopiesAvailableInStock)
        {
            Notificator.Handle("It is not possible to update a book that has a borrowed or renewed copy");
            return false;
        }

        var book = Mapper.Map<Book>(dto);
        var validator = new BookValidator();

        var result = await validator.ValidateAsync(book);
        if (!result.IsValid)
        {
            Notificator.Handle(result.Errors);
            return false;
        }

        return true;
    }

    private bool IsImage(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }

    private async Task<int> GenerateCode()
    {
        var lastCode = await _bookRepository.Queryable()
            .OrderByDescending(b => b.Code)
            .Select(b => b.Code)
            .FirstOrDefaultAsync();

        return lastCode == 0 ? 1000 : lastCode + 1;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _bookRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("An error occurred while saving changes");
        return false;
    }
}