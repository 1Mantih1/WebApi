namespace WebApi.Dto
{
    public class AuthorResponseDto
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
