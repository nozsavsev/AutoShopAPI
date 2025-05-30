using AutoShopAPI.DbContexts;
using AutoShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoShopAPI.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AutoShopDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<User>> GetAllAsync(int? skip = null, int? take = null)
        {
            IQueryable<User> query = _dbSet;
            if (skip.HasValue && take.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }
            else if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            else if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            query = query.Include(u => u.Car);

            return await query.ToListAsync();
        }

        public async Task<User?> GetUserWithCarAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Car)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
