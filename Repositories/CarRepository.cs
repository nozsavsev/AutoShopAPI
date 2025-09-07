using AutoShopAPI.DbContexts;
using AutoShopAPI.Models;
using AutoShopAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AutoShopAPI.Repositories
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository(AutoShopDbContext context) : base(context)
        {
        }

        public async Task<bool> HasAnyUsersAsync(int carId)
        {
            return await _context.Users.AnyAsync(u => u.CarId == carId);
        }



        private IQueryable<Car> ConfigureFindCarQuery(SearchCarFilters filters, QueryCallback<Car>? queryCallback = null)
        {
            var query = BuildQuery(queryCallback);


            if (!string.IsNullOrWhiteSpace(filters.TextMatch))
            {
                var trimmed = filters.TextMatch!.Trim();
                var text = $"%{trimmed}%";

                if (int.TryParse(trimmed, out var idMatch))
                {
                    query = query.Where(u =>
                        u.Id == idMatch ||
                        EF.Functions.ILike(u.Company, text) ||
                        EF.Functions.ILike(u.Model, text)
                    );
                }
                else
                {
                    query = query.Where(u =>
                        EF.Functions.ILike(u.Company, text) ||
                        EF.Functions.ILike(u.Model, text)
                    );
                }
            }

            switch (filters.SortBy)
            {
                case CarSortBy.ModelAsc:
                query = query.OrderBy(u => u.Model);
                break;
                case CarSortBy.ModelDesc:
                query = query.OrderByDescending(u => u.Model);
                break;

                case CarSortBy.CompanyAsc:
                query = query.OrderBy(u => u.Company);
                break;
                case CarSortBy.CompanyDesc:
                query = query.OrderByDescending(u => u.Company);
                break;

                case CarSortBy.CreatedAtAsc:
                query = query.OrderBy(u => u.CreatedAt);
                break;
                case CarSortBy.CreatedAtDesc:
                query = query.OrderByDescending(u => u.CreatedAt);
                break;

                case CarSortBy.UpdatedAtAsc:
                query = query.OrderBy(u => u.UpdatedAt);
                break;
                case CarSortBy.UpdatedAtDesc:
                query = query.OrderByDescending(u => u.UpdatedAt);
                break;

            }

            return query;
        }

        public async Task<IEnumerable<Car>> FindCarsAsync(SearchCarFilters filters, QueryCallback<Car>? queryCallback = null)
        {
            var query = ConfigureFindCarQuery(filters, queryCallback);

            return await query
                .Skip(filters.Skip ?? 0)
                .Take(filters.Take ?? 10)
                .ToListAsync();
        }

        public async Task<int> CountFoundCarsAsync(SearchCarFilters filters)
        {

            var query = ConfigureFindCarQuery(filters);

            return await query.CountAsync();
        }
    }
}