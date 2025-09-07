using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AutoShopAPI.Models.DTOs
{
    public class AllUsersDTO
    {
        public IEnumerable<UserDTO> Users { get; set; } = null!;
        public long TotalCount { get; set; }
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserSortBy
    {
        NameAsc,

        NameDesc,

        EmailAsc,

        EmailDesc,

        CreatedAtAsc,

        CreatedAtDesc,

        UpdatedAtAsc,

        UpdatedAtDesc
    }

    public class SearchUserFilters
    {

        public static int MaxTakeValue = 100;
        [StringLength(100)]
        public string? TextMatch { get; set; } = null!;
        [Range(0, int.MaxValue)]
        public int? Skip { get; set; } = 0;
        [Range(1, 100)]
        public int? Take { get; set; } = 10;
        public UserSortBy? SortBy { get; set; } = UserSortBy.NameAsc;

        public SearchUserFilters VerifyAndFix()
        {
            Skip = Skip < 0 ? 0 : Skip;
            Take = Take < 1 ? 1 : Take > MaxTakeValue ? MaxTakeValue : Take;

            SortBy = SortBy ?? UserSortBy.NameAsc;

            return this;
        }
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CarBasicDTO? Car { get; set; }
    }

    public class CreateUserDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;
        public int? CarId { get; set; }
    }

    public class UpdateUserDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }
        public int? CarId { get; set; }
    }

    //version without car to be used in UserDTO to avoid circular deps
    public class UserBasicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}

