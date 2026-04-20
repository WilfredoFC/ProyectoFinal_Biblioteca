using ProyectoFinal_Biblioteca.ViewModels;

namespace ProyectoFinal_Biblioteca.Views;

public partial class StatisticsPage : ContentPage
{
	public StatisticsPage(StatisticsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}