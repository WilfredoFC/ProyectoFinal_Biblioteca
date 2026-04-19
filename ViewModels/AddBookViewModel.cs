using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoFinal_Biblioteca.Models;
using ProyectoFinal_Biblioteca.Services;

namespace ProyectoFinal_Biblioteca.ViewModels
{

    public partial class AddBookViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private Book book = new();

        public AddBookViewModel(DatabaseService db) => _db = db;

        [RelayCommand]
        private async Task SaveBook()
        {
            if (string.IsNullOrWhiteSpace(Book.Title))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El título es obligatorio", "OK");
                return;
            }
            if (Book.Rating < 0 || Book.Rating > 5)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Rating debe ser entre 1 y 5", "OK");
                return;
            }
            await _db.SaveBookAsync(Book);
            await Shell.Current.GoToAsync("..");
        }
    }
}