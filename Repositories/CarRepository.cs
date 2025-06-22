using AutoShopAPI.DbContexts;
using AutoShopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace AutoShopAPI.Repositories
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository(AutoShopDbContext context) : base(context)
        {
        }


        public override async Task<IEnumerable<Car>> GetAllAsync(int? skip = null, int? take = null)
        {
            IQueryable<Car> query = _dbSet;

            query.OrderBy(c => c.Id);

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

            query = query.Include(u => u.Users);

            return await query.ToListAsync();
        }

        public async Task<bool> HasAssignedUsersAsync(int carId)
        {
            return await _context.Users.AnyAsync(u => u.CarId == carId);
        }

        public override async Task<IEnumerable<Car>> FindAsync(Expression<Func<Car, bool>> expression)
        {
            return await _dbSet.Where(expression).Include(u => u.Users).ToListAsync();
        }

        public override async Task<Car?> GetByIdAsync(int id)
        {
            return await _dbSet.Where(u => u.Id == id).Include(u => u.Users).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Car>> FindCars(string? textMatch, int? skip = null, int? take = null)
        {
            var query = _dbSet
            .Include(u => u.Users).AsQueryable();
            
            if (textMatch != null)
                query = query.Where(c => 
                c.Company.ToLower().Contains(textMatch.ToLower()) || 
                c.Model.ToLower().Contains(textMatch.ToLower()) || 
                (c.Users != null && c.Users.Any(u => u.Name.ToLower().Contains(textMatch.ToLower()) || u.Email.ToLower().Contains(textMatch.ToLower()))));
            
            return await query.OrderBy(u => u.Id)
                 .Skip(skip ?? 0)
                 .Take(take ?? int.MaxValue).ToListAsync();
        }

        public async Task<int> CountFoundCars(string? textMatch)
        {
            var query = _dbSet
               .Include(u => u.Users).AsQueryable();
            if (textMatch != null)
                query = query.Where(c => 
                c.Company.ToLower().Contains(textMatch.ToLower()) ||
                c.Model.ToLower().Contains(textMatch.ToLower()) ||
                (c.Users != null && c.Users.Any(u => u.Name.ToLower().Contains(textMatch.ToLower()) || u.Email.ToLower().Contains(textMatch.ToLower()))));
            return await query.CountAsync();

        }
    }
}