namespace WebApi.Dto
{
    public class BookResponseDto
    {
        public int BookID { get; set; }
        public string Title { get; set; } = null!;
        public int AuthorId { get; set; }
        public string ISBN { get; set; } = null!;
        public decimal Price { get; set; }
        public int Pages { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }
}
