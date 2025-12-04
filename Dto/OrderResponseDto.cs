namespace WebApi.Dto
{
    public class OrderResponseDto
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string CustomerEmail { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string? ShippingAddress { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; } = null!;
        public List<OrderItemResponseDto> OrderItems { get; set; } = new List<OrderItemResponseDto>();
    }
}

