using BookApi.Data;
using BookApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Repository
{
    public class BookApiRepository : IBookApiRepository
    {
        private readonly ILogger<BookApiRepository> _logger;
        private readonly BookContext _context;
        public BookApiRepository(ILogger<BookApiRepository> logger, BookContext context) 
        {
            _logger = logger;   
            _context = context;
        }
        public async Task<IEnumerable<Book>> SearchBookAsync(string searchString)
        {
            if (_context.Books == null)
            {
                throw new ArgumentNullException($"{nameof(SearchBookAsync)} Books null");
            }

            var books = from f in _context.Books
                         select f;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => (s.Title!.Contains(searchString)) || (s.Author!.Contains(searchString)) || (s.Genre!.Contains(searchString)));
            }

            return await books.ToListAsync();

        }
    }
}
