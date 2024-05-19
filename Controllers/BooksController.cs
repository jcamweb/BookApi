using BookApi.Data;
using BookApi.Models;
using BookApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookContext _context;
        private readonly IBookApiRepository _repository;
        private readonly ILogger<Book> _logger;

        public BooksController(BookContext context, IBookApiRepository repository, ILogger<Book> logger)
        {
            _context = context;
            _repository = repository;
            _logger = logger;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Devuelve books coinciden en Title, Author o Genre con parámetro de búsqueda
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>books</returns>

        [HttpGet("search")]
        public async Task<IActionResult> SearchPharma([FromQuery(Name = "searchString")] string searchstring)
        {
            var books = await _repository.SearchBookAsync(searchstring);
            if (books == null)
            {
                _logger.LogError($"Sin resultados en búsqueda de {searchstring}");
                return NotFound();
            }

            return Ok(books);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}

