namespace ProyectoFinal_Biblioteca
{
    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPage()
        {
            InitializeComponent();
            BindingContext = new ViewModel.StatisticsViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ViewModel.StatisticsViewModel vm)
            {
                await vm.LoadStatisticsAsync();

                PieDrawable.ReadCount = vm.ReadCount;
                PieDrawable.PendingCount = vm.PendingCount;
                BarDrawable.Data = vm.GenreData;
            }

            PieChartView.Invalidate();
            BarChartView.Invalidate();
        }
    }
}
