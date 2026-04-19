using ProyectoFinal_Biblioteca.ViewModels;

namespace ProyectoFinal_Biblioteca.Views
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage(SearchViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}