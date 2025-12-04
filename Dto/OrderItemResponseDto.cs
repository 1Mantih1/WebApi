namespace WebApi.Dto
{
    public class OrderItemResponseDto
    {
        public int OrderItemID { get; set; }
        public int BookID { get; set; }
        public string BookTitle { get; set; } = null!;
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
    }
}

