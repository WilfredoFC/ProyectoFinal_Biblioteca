using ProyectoFinal_Biblioteca.ViewModels;

namespace ProyectoFinal_Biblioteca.Views;

public partial class LibraryPage : ContentPage
{
	public LibraryPage(LibraryViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}