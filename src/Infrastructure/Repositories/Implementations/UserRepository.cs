using Domain.Entities;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }
        public IQueryable<User> GetUsersQuery(string search = null)
        {
            var query = _context.Users.IgnoreQueryFilters() as IQueryable<User>;

            if (!string.IsNullOrEmpty(search))
            {
                var searchQuery = search.Trim();
                query = query.Where(x =>
                x.UserName.Contains(searchQuery) ||
                x.PhoneNumber.Contains(searchQuery));
            }

            return query;
        }
    }
}

