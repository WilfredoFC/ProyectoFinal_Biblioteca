using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoFinal_Biblioteca.Models;
using ProyectoFinal_Biblioteca.Services;
using System.Collections.ObjectModel;

namespace ProyectoFinal_Biblioteca.ViewModels
{

    public partial class LibraryViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private ObservableCollection<Book> books = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string selectedFilter = "Todos";

        public List<string> FilterOptions { get; } = new() { "Todos", "Leídos", "Pendientes" };

        public LibraryViewModel(DatabaseService db)
        {
            _db = db;
            LoadBooksCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadBooks()
        {
            var allBooks = await _db.GetBooksAsync();
            ApplyFilterAndSearch(allBooks);
        }

        // Se ejecuta cuando cambia SearchText o SelectedFilter
        partial void OnSearchTextChanged(string value)
        {
            _ = LoadBooks();
        }

        partial void OnSelectedFilterChanged(string value)
        {
            _ = LoadBooks();
        }

        private void ApplyFilterAndSearch(List<Book> source)
        {
            IEnumerable<Book> filtered = source;

            if (SelectedFilter == "Leídos")
                filtered = filtered.Where(b => b.IsRead);
            else if (SelectedFilter == "Pendientes")
                filtered = filtered.Where(b => !b.IsRead);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(b =>
                    b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            Books = new ObservableCollection<Book>(filtered);
        }

        [RelayCommand]
        private async Task DeleteBook(Book book)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Eliminar", $"¿Borrar '{book.Title}'?", "Sí", "No");
            if (confirm)
            {
                await _db.DeleteBookAsync(book);
                await LoadBooks();
            }
        }

        [RelayCommand]
        private async Task ToggleReadStatus(Book book)
        {
            await _db.UpdateReadStatusAsync(book.Id, !book.IsRead);
            await LoadBooks();
        }

        [RelayCommand]
        private async Task GoToAddBook() => await Shell.Current.GoToAsync("addbook");

        [RelayCommand]
        private async Task GoToDetails(Book book)
        {
            var parameters = new Dictionary<string, object> { { "book", book } };
            await Shell.Current.GoToAsync("bookdetail", parameters);
        }
    }
}