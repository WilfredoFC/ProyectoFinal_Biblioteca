using ProyectoFinal_Biblioteca.Models;
using ProyectoFinal_Biblioteca.Services;
using ProyectoFinal_Biblioteca.ViewModels;

namespace ProyectoFinal_Biblioteca.Views
{
    public partial class BookDetailPage : ContentPage
    {
        private readonly BookDetailViewModel _viewModel;

        public BookDetailPage(BookDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Obtener el libro del servicio de navegación
            var book = NavigationService.Instance.SelectedBook;
            if (book != null && _viewModel != null)
            {
                _viewModel.Book = book;
            }
        }
    }
}