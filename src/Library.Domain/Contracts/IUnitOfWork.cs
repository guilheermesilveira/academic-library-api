namespace Library.Domain.Contracts;

public interface IUnitOfWork
{
    Task<bool> Commit();
}