using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ProyectoFinal_Biblioteca.Models;
using ProyectoFinal_Biblioteca.Services;

namespace ProyectoFinal_Biblioteca.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly BookApiService _api;
        private readonly DatabaseService _db;

        [ObservableProperty]
        private string query = string.Empty;

        [ObservableProperty]
        private ObservableCollection<BookSearchResult> results = new();

        [ObservableProperty]
        private bool isBusy;

        public SearchViewModel(BookApiService api, DatabaseService db)
        {
            _api = api;
            _db = db;
        }

        [RelayCommand]
        private async Task SearchBooks()
        {
            if (string.IsNullOrWhiteSpace(Query)) return;
            IsBusy = true;
            try
            {
                var list = await _api.SearchBooksAsync(Query);
                Results = new ObservableCollection<BookSearchResult>(list);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task AddToLibrary(BookSearchResult result)
        {
            var detail = await _api.GetBookDetailAsync(result.Id);
            if (detail == null) return;

            var book = new Book
            {
                Title = detail.Title,
                Author = detail.Author,
                Genre = detail.Categories.FirstOrDefault() ?? "Sin género",
                Isbn = detail.Isbn,
                PublishedYear = detail.PublishedYear,
                PageCount = detail.PageCount,
                Description = detail.Description,
                CoverUrl = detail.CoverUrl,
                IsRead = false,
                Rating = 0
            };
            await _db.SaveBookAsync(book);
            await Application.Current.MainPage.DisplayAlert("Éxito", "Libro agregado a tu biblioteca", "OK");
        }
    }
}