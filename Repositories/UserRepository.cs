using AutoShopAPI.DbContexts;
using AutoShopAPI.Models;
using AutoShopAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;


namespace AutoShopAPI.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AutoShopDbContext context) : base(context)
        {

        }

        public Task<User?> GetByEmailAsync(string email, QueryCallback<User>? queryCallback = null)
        {
            var query = BuildQuery(queryCallback);

            return query
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        private IQueryable<User> ConfigureFindUserQuery(SearchUserFilters filters, QueryCallback<User>? queryCallback = null)
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
                        EF.Functions.ILike(u.Name, text) ||
                        EF.Functions.ILike(u.Email, text)
                    );
                }
                else
                {
                    query = query.Where(u =>
                        EF.Functions.ILike(u.Name, text) ||
                        EF.Functions.ILike(u.Email, text)
                    );
                }
            }

            switch (filters.SortBy)
            {
                case UserSortBy.NameAsc:
                    query = query.OrderBy(u => u.Name);
                    break;
                case UserSortBy.NameDesc:
                    query = query.OrderByDescending(u => u.Name);
                    break;
                case UserSortBy.EmailAsc:
                    query = query.OrderBy(u => u.Email);
                    break;
                case UserSortBy.EmailDesc:
                    query = query.OrderByDescending(u => u.Email);
                    break;
                case UserSortBy.CreatedAtAsc:
                    query = query.OrderBy(u => u.CreatedAt);
                    break;
                case UserSortBy.CreatedAtDesc:
                    query = query.OrderByDescending(u => u.CreatedAt);
                    break;
                case UserSortBy.UpdatedAtAsc:
                    query = query.OrderBy(u => u.UpdatedAt);
                    break;
                case UserSortBy.UpdatedAtDesc:
                    query = query.OrderByDescending(u => u.UpdatedAt);
                    break;
            }

            return query;
        }

        public async Task<IEnumerable<User>> FindUsersAsync(SearchUserFilters filters, QueryCallback<User>? queryCallback = null)
        {
            var query = ConfigureFindUserQuery(filters, queryCallback);
            return await query
                .Skip(filters.Skip ?? 0)
                .Take(filters.Take ?? 10)
                .ToListAsync();
        }

        public async Task<int> CountFoundUsersAsync(SearchUserFilters filters)
        {
            var query = ConfigureFindUserQuery(filters);
            return await query.CountAsync();
        }
    }
}
