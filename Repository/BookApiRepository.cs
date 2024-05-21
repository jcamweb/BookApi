using BookApi.Data;
using BookApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Books.FindAsync(id);
            }

            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetByIdAsync)} no ha podido recuperar book: {ex.Message}");
                throw new Exception($"{nameof(GetByIdAsync)} no ha podido recuperar book: {ex.Message}");

            }

        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            try 
            { 
            var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteAsync)} no ha podido recuperar book: {ex.Message}");
                throw new Exception($"{nameof(DeleteAsync)} no ha podido recuperar book: {ex.Message}");

            }

        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }
        public async Task<IEnumerable<Book>> SearchBookAsync(string searchString)
        {
            
            if (_context.Books == null)
            {
                throw new ArgumentNullException($"{nameof(SearchBookAsync)} Books null");
            }
                      
            var books = from f in _context.Books
                         select f;
            searchString = searchString.ToUpper();
            Task tasksearch = new Task(() => books = books.Where(s => (s.Title.ToUpper()!.Contains(searchString)) || (s.Author.ToUpper()!.Contains(searchString)) || (s.Genre.ToUpper()!.Contains(searchString))));
            if (!String.IsNullOrEmpty(searchString))
            {
                tasksearch.Start();
                             
            }
         
             tasksearch.Wait();
             return await books.ToListAsync();                

        }
    }
}
