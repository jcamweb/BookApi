using BookApi.Models;

namespace BookApi.Repository
{
    public interface IBookApiRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Book>> SearchBookAsync(string search);

    }
}
