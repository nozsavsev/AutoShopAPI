using AutoShopAPI.DbContexts;
using AutoShopAPI.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Car?> GetCarWithUsersAsync(int carId)
        {
            return await _context.Cars
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == carId);
        }
    }
}
