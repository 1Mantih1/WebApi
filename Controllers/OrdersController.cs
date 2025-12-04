using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly BookstoreContext _db;
        private readonly IMapper _mapper;

        public OrdersController(BookstoreContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int? customerId = null)
        {
            var currentUser = await _db.Users
                .SingleOrDefaultAsync(x => x.Email == User.Identity!.Name);

            if (currentUser == null)
                return Unauthorized();

            var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            var query = _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .AsQueryable();

            if (!isAdminOrManager)
            {
                query = query.Where(o => o.CustomerId == currentUser.UserId);
            }
            else if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }

            var orders = await query
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var responseDtos = orders.Select(order => _mapper.Map<OrderResponseDto>(order)).ToList();

            return Ok(responseDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var currentUser = await _db.Users
                .SingleOrDefaultAsync(x => x.Email == User.Identity!.Name);

            if (currentUser == null)
                return Unauthorized();

            var order = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");
            if (!isAdminOrManager && order.CustomerId != currentUser.UserId)
                return Forbid();

            return Ok(_mapper.Map<OrderResponseDto>(order));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _db.Users
                .SingleOrDefaultAsync(x => x.Email == User.Identity!.Name);

            if (currentUser == null)
                return Unauthorized();

            var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            if (!isAdminOrManager && dto.CustomerID != currentUser.UserId)
                return Forbid();

            var customer = await _db.Users.FindAsync(dto.CustomerID);
            if (customer == null)
                return BadRequest("Customer not found");

            var status = await _db.Statuses.FindAsync(dto.StatusID);
            if (status == null)
                return BadRequest("Status not found");

            var bookIds = dto.OrderItems.Select(oi => oi.BookID).ToList();
            var books = await _db.Books
                .Where(b => bookIds.Contains(b.BookId))
                .ToListAsync();

            if (books.Count != bookIds.Count)
                return BadRequest("One or more books not found");

            foreach (var orderItemDto in dto.OrderItems)
            {
                var book = books.First(b => b.BookId == orderItemDto.BookID);
                if (!book.IsAvailable)
                    return BadRequest($"Book '{book.Title}' is not available");

                if (book.StockQuantity < orderItemDto.Quantity)
                    return BadRequest($"Insufficient stock for book '{book.Title}'. Available: {book.StockQuantity}, Requested: {orderItemDto.Quantity}");
            }

            var order = _mapper.Map<Order>(dto);
            order.OrderDate = DateTime.UtcNow;

            _db.Orders.Add(order);

            try
            {
                await _db.SaveChangesAsync();

                foreach (var orderItemDto in dto.OrderItems)
                {
                    var orderItem = _mapper.Map<OrderItem>(orderItemDto);
                    orderItem.OrderId = order.OrderId;
                    _db.OrderItems.Add(orderItem);

                    var book = books.First(b => b.BookId == orderItemDto.BookID);
                    book.StockQuantity -= orderItemDto.Quantity;
                    if (book.StockQuantity == 0)
                        book.IsAvailable = false;
                }

                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving order: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while saving the order.");
            }

            var createdOrder = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            
            if (createdOrder == null)
                return StatusCode(500, "Order was created but could not be retrieved.");

            var responseDto = _mapper.Map<OrderResponseDto>(createdOrder);
            return CreatedAtAction(nameof(GetOrder), new { id = responseDto.OrderID }, responseDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var currentUser = await _db.Users
                .SingleOrDefaultAsync(x => x.Email == User.Identity!.Name);

            if (currentUser == null)
                return Unauthorized();

            var order = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            if (!isAdminOrManager)
            {
                if (order == null || order.CustomerId != currentUser.UserId)
                    return Forbid();
            }

            if (order == null)
                return NotFound();

            foreach (var orderItem in order.OrderItems)
            {
                var book = await _db.Books.FindAsync(orderItem.BookId);
                if (book != null)
                {
                    book.StockQuantity += orderItem.Quantity;
                    if (book.StockQuantity > 0 && !book.IsAvailable)
                        book.IsAvailable = true;
                }
            }

            try
            {
                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error deleting order: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while deleting the order.");
            }

            return Ok(new { message = "Order deleted successfully", orderId = id });
        }
    }
}

