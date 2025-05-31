using AutoShopAPI.DbContexts;
using AutoShopAPI.Models;
using Microsoft.EntityFrameworkCore;
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
    }
}
