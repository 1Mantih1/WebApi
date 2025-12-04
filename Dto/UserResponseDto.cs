namespace WebApi.Dto
{
    public class UserResponseDto
    {
        public int UserID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } 
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; } = null!;
    }
}