using Library.Domain.Contracts.Repositories;
using Library.Domain.Entities;
using Library.Infra.Data.Context;

namespace Library.Infra.Data.Repositories;

public class AdministratorRepository : Repository<Administrator>, IAdministratorRepository
{
    public AdministratorRepository(ApplicationDbContext context) : base(context)
    {
    }
}