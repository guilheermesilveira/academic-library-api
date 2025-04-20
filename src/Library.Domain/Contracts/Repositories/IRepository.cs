using System.Linq.Expressions;
using Library.Domain.Entities;

namespace Library.Domain.Contracts.Repositories;

public interface IRepository<T> : IDisposable where T : Entity
{
    IUnitOfWork UnitOfWork { get; }
    Task<T?> FirstOrDefault(Expression<Func<T, bool>> expression);
}