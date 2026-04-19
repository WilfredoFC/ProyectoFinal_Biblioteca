using ProyectoFinal_Biblioteca.Models;
using SQLite;

namespace ProyectoFinal_Biblioteca.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "biblioteca.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Book>().Wait();
        }

        public Task<int> SaveBookAsync(Book book)
        {
            if (book.Id == 0)
                return _database.InsertAsync(book);
            else
                return _database.UpdateAsync(book);
        }

        public Task<List<Book>> GetBooksAsync() => _database.Table<Book>().ToListAsync();

        public Task<List<Book>> GetBooksByGenreAsync(string genre) =>
            _database.Table<Book>().Where(b => b.Genre == genre).ToListAsync();

        public Task<List<Book>> GetReadBooksAsync() =>
            _database.Table<Book>().Where(b => b.IsRead).ToListAsync();

        public Task<List<Book>> GetUnreadBooksAsync() =>
            _database.Table<Book>().Where(b => !b.IsRead).ToListAsync();

        public Task<int> DeleteBookAsync(Book book) => _database.DeleteAsync(book);

        public Task<int> UpdateReadStatusAsync(int bookId, bool isRead)
        {
            var book = _database.FindAsync<Book>(bookId).Result;
            if (book != null)
            {
                book.IsRead = isRead;
                return _database.UpdateAsync(book);
            }
            return Task.FromResult(0);
        }

        public async Task<(int Total, int Read, int Unread)> GetStatisticsAsync()
        {
            var all = await GetBooksAsync();
            var read = all.Count(b => b.IsRead);
            return (all.Count, read, all.Count - read);
        }
    }
}