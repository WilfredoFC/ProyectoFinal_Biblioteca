using ProyectoFinal_Biblioteca.ViewModels;

namespace ProyectoFinal_Biblioteca.Views
{

    public partial class BookDetailPage : ContentPage
    {
        public BookDetailPage(BookDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}