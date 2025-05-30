namespace AutoShopAPI.Models.DTOs
{
    public class AllUsersDTO
    {
        public IEnumerable<UserDTO> Users { get; set; } = null!;
        public long TotalCount { get; set; }
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

    public class CreateUpdateUserDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
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

