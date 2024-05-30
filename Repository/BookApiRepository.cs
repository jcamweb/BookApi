using BookApi.Data;
using BookApi.Extensions;
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
            Task tasknormalize = new Task(() =>
            { 
            foreach (var book in books)
            {
                book.Author = book.Author.NormalizeString();
                book.Title = book.Title.NormalizeString();
                book.Genre = book.Genre.NormalizeString();
                book.PublishedYear = book.PublishedYear.NormalizeString();
                

            }
            }
            );
            searchString = searchString.NormalizeString();
            tasknormalize.Start();
            tasknormalize.Wait();    
           // Task tasksearch = new Task(() => books = books.Where(s => (EF.Functions.Like(s.Title, searchString)))); //case search only one field s.Title==(searchString) / EF.Functions.Like(s.Title, searchString)
            Task tasksearch = new Task(() => books = books.Where(s => (s.Title!.Contains(searchString)) || (s.Author!.Contains(searchString)) || (s.Genre!.Contains(searchString))));
            if (!String.IsNullOrEmpty(searchString))
            {
               
                tasksearch.Start();
                             
            }
             
             tasksearch.Wait();
               // case use direct SQL
            //var blogs = _context.Books
            //         .FromSql($"SELECT * FROM [Books] WHERE Title = {searchString}")
            //         .ToListAsync();
            //  return await blogs;
            return await books.ToListAsync();
          

        }
    }
}
