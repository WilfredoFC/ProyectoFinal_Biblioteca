using ProyectoFinal_Biblioteca.ViewModels;

namespace ProyectoFinal_Biblioteca.Views
{
    public partial class AddBookPage : ContentPage
    {
        public AddBookPage(AddBookViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}