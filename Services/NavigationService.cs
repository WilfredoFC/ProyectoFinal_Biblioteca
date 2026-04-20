using ProyectoFinal_Biblioteca.Models;

namespace ProyectoFinal_Biblioteca.Services
{
    /// <summary>
    /// Servicio para pasar datos entre páginas durante la navegación
    /// </summary>
    public class NavigationService
    {
        private static NavigationService _instance;
        private Book _selectedBook;

        public static NavigationService Instance => _instance ??= new NavigationService();

        public Book SelectedBook
        {
            get => _selectedBook;
            set => _selectedBook = value;
        }
    }
}
