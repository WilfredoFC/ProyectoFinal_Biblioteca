namespace ProyectoFinal_Biblioteca.ViewModel;

public class StatisticsViewModel : BaseViewModel
{
    public StatisticsViewModel()
    {
        _genreData = new Dictionary<string, int>();
        LoadStatisticsCommand = new Command(async () => await LoadStatisticsAsync());
    }

    public Command LoadStatisticsCommand { get; }

    // Datos para el Gráfico Circular
    private int _readCount;
    public int ReadCount { get => _readCount; set => SetProperty(ref _readCount, value); }

    private int _pendingCount;
    public int PendingCount { get => _pendingCount; set => SetProperty(ref _pendingCount, value); }

    // Datos para el Gráfico de Barras
    private Dictionary<string, int> _genreData;
    public Dictionary<string, int> GenreData { get => _genreData; set => SetProperty(ref _genreData, value); }

    // Total de libros
    private int _totalBooks;
    public int TotalBooks { get => _totalBooks; set => SetProperty(ref _totalBooks, value); }

    public async Task LoadStatisticsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await Task.CompletedTask;

            TotalBooks = 0;
            ReadCount = 0;
            PendingCount = 0;
            GenreData = new Dictionary<string, int>();
        }
        finally
        {
            IsBusy = false;
        }
    }
}