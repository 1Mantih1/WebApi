namespace WebApi.Dto
{
    public class TokenResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}
