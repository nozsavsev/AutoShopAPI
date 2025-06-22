using AutoShopAPI.DbContexts;
using AutoShopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

            query = query.OrderBy(u => u.Id);

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

        public override async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> expression)
        {
            return await _dbSet.Where(expression).Include(u => u.Car).ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet.Where(u => u.Id == id).Include(u => u.Car).FirstOrDefaultAsync();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _dbSet
                .Where(u => u.Email == email)
                .Include(u => u.Car)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> FindUsers(string? textMatch, int? skip = null, int? take = null)
        {
            var query = _dbSet
                .Include(u => u.Car).AsQueryable();

            if (textMatch != null)
                query = query.Where(u =>
                 (u.Name.ToLower().Contains(textMatch.ToLower())) ||
                 (u.Email.ToLower().Contains(textMatch.ToLower())) ||
                 (u.Car != null && u.Car.Company.ToLower().Contains(textMatch.ToLower())) ||
                 (u.Car != null && u.Car.Model.ToLower().Contains(textMatch.ToLower()))
                 );

            return await query.OrderBy(u => u.Id)
                .Skip(skip ?? 0)
                .Take(take ?? int.MaxValue).ToListAsync();
        }

        public async Task<int> CountFoundUsers(string? textMatch)
        {
            var query = _dbSet
                 .Include(u => u.Car).AsQueryable();
            if (textMatch != null)
                query = query.Where(u => 
                u.Name.ToLower().Contains(textMatch.ToLower()) || 
                u.Email.ToLower().Contains(textMatch.ToLower()) || 
                (u.Car != null && u.Car.Company.ToLower().Contains(textMatch.ToLower())) ||
                (u.Car != null && u.Car.Model.ToLower().Contains(textMatch.ToLower()))

                );

            return await query.CountAsync();
        }
    }
}
