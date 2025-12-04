namespace WebApi.Dto
{
    public class BookUpdateDto
    {
        public string? Title { get; set; }
        public int? AuthorId { get; set; }
        public string? ISBN { get; set; }
        public decimal? Price { get; set; }
        public int? Pages { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
