using Microsoft.EntityFrameworkCore;
using WebApplication2.DbContexts;
using WebApplication2.Entities;

namespace WebApplication2.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernamaeWithRole(string email);
    Task<User?> GetByIdWithRoleAsync(int userId);
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(
        TestdbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<User?> GetByIdWithRoleAsync(int userId)
    {
        return await _dbSet
            .Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<User?> GetByUsernamaeWithRole(string email)
    {
        return await _dbSet
            .Include(x => x.Role)
            .SingleOrDefaultAsync(u => u.Email == email);
    }
}
