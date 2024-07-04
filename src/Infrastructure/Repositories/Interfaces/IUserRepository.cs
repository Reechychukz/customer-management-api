using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        IQueryable<User> GetUsersQuery(string search = null);
    }
}

