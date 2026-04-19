using CommunityToolkit.Mvvm.ComponentModel;
using ProyectoFinal_Biblioteca.Drawables;
using ProyectoFinal_Biblioteca.Services;

namespace ProyectoFinal_Biblioteca.ViewModels
{

    public partial class StatisticsViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private StatisticsDrawable statisticsDrawable = new();

        public StatisticsViewModel(DatabaseService db)
        {
            _db = db;
            LoadStatistics();
        }

        private async void LoadStatistics()
        {
            var books = await _db.GetBooksAsync();
            var total = books.Count;
            var read = books.Count(b => b.IsRead);
            var unread = total - read;
            var byGenre = books.GroupBy(b => b.Genre).ToDictionary(g => g.Key, g => g.Count());

            StatisticsDrawable = new StatisticsDrawable
            {
                TotalBooks = total,
                ReadBooks = read,
                UnreadBooks = unread,
                BooksByGenre = byGenre
            };
        }
    }
}