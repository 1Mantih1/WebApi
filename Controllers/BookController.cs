using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookstoreContext _db;
        private readonly IMapper _mapper;

        public BookController(BookstoreContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(
            [FromQuery] int? authorId = null,
            [FromQuery] string? titleSearch = null)
        {
            var query = _db.Books.AsQueryable();

            if (authorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(titleSearch))
            {
                query = query.Where(b => b.Title.Contains(titleSearch));
            }

            var books = await query
                .AsNoTracking()
                .Select(x => _mapper.Map<BookResponseDto>(x))
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            return Ok(_mapper.Map<BookResponseDto>(book));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = _mapper.Map<Book>(dto);
            _db.Books.Add(book);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving book: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while saving the book.");
            }

            var responseDto = _mapper.Map<BookResponseDto>(book);
            return CreatedAtAction(nameof(GetBook), new { id = responseDto.BookID }, responseDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingBook = await _db.Books.FindAsync(id);
            if (existingBook == null)
                return NotFound();

            _mapper.Map(dto, existingBook);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating book: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while updating the book.");
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            try
            {
                _db.Books.Remove(book);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while deleting the book.");
            }

            return NoContent();
        }
    }
}