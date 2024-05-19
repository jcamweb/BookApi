using BookApi.Models;

namespace BookApi.Repository
{
    public interface IBookApiRepository
    {
        Task<IEnumerable<Book>> SearchBookAsync(string search);

    }
}
