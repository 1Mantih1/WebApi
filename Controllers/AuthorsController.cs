using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly BookstoreContext _db;
        private readonly IMapper _mapper;

        public AuthorsController(BookstoreContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _db.Authors
                .AsNoTracking()
                .Select(x => _mapper.Map<AuthorResponseDto>(x))
                .ToListAsync();
            return Ok(authors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            return Ok(_mapper.Map<AuthorResponseDto>(author));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var author = _mapper.Map<Author>(dto);
            _db.Authors.Add(author);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving author: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while saving the author.");
            }

            var responseDto = _mapper.Map<AuthorResponseDto>(author);
            return CreatedAtAction(nameof(GetAuthor), new { id = responseDto.AuthorId }, responseDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingAuthor = await _db.Authors.FindAsync(id);
            if (existingAuthor == null)
                return NotFound();

            _mapper.Map(dto, existingAuthor);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating author: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while updating the author.");
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            try
            {
                _db.Authors.Remove(author);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error deleting author: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                return StatusCode(500, "An error occurred while deleting the author.");
            }

            return NoContent();
        }
    }
}