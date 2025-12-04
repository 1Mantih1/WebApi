namespace WebApi.Dto
{
    public class OrderCreateDto
    {
        public int CustomerID { get; set; }
        public string? ShippingAddress { get; set; }
        public int StatusID { get; set; }
        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
    }
}

