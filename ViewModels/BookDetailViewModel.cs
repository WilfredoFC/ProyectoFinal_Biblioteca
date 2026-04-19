using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoFinal_Biblioteca.Models;
using ProyectoFinal_Biblioteca.Services;

namespace ProyectoFinal_Biblioteca.ViewModels
{

    public partial class BookDetailViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private Book book = new();

        public BookDetailViewModel(DatabaseService db) => _db = db;

        [RelayCommand]
        private async Task SaveChanges()
        {
            await _db.SaveBookAsync(Book);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task DeleteBook()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Eliminar", "¿Borrar libro?", "Sí", "No");
            if (confirm)
            {
                await _db.DeleteBookAsync(Book);
                await Shell.Current.GoToAsync("//library");
            }
        }
    }
}