
using static AutoShopAPI.Models.DTOs.UserDTO;

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

    public class CreateUpdateCarDTO
    {
        public string Company { get; set; } = null!;
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
