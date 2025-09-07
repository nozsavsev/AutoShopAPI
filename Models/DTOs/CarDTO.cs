using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AutoShopAPI.Models.DTOs
{

    public class AllCarsDTO
    {
        public IEnumerable<CarDTO> Cars { get; set; } = null!;
        public long TotalCount { get; set; }
    }

    public class CarDTO
    {
       
            public int Id { get; set; }
            public string Company { get; set; } = null!;
            public string Model { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public ICollection<UserBasicDTO>? Users { get; set; }
        

    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CarSortBy
    {
        CompanyAsc,

        CompanyDesc,

        ModelAsc,

        ModelDesc,

        CreatedAtAsc,

        CreatedAtDesc,

        UpdatedAtAsc,

        UpdatedAtDesc
    }

    public class SearchCarFilters
    {

        public static int MaxTakeValue = 100;
        [StringLength(100)]
        public string? TextMatch { get; set; } = null;
        [Range(0, int.MaxValue)]
        public int? Skip { get; set; } = 0;
        [Range(1, 100)]
        public int? Take { get; set; } = 10;
        [Range(0, int.MaxValue)]
        public CarSortBy? SortBy { get; set; } = CarSortBy.CompanyAsc;
        
        public SearchCarFilters VerifyAndFix()
        {
            Skip = Skip < 0 ? 0 : Skip;
            Take = Take < 1 ? 1 : Take > MaxTakeValue ? MaxTakeValue : Take;


            SortBy = SortBy ?? CarSortBy.CompanyAsc;

            return this;
        }
    }
    public class CreateUpdateCarDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Company { get; set; } = null!;
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Model { get; set; } = null!;
    }

    //version without user to be used in UserDTO to avoid circular deps
    public class CarBasicDTO
    {
        public int Id { get; set; }
        public string Company { get; set; } = null!;
        public string Model { get; set; } = null!;
    }
}
