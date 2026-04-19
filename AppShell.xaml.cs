using ProyectoFinal_Biblioteca.Views;

namespace ProyectoFinal_Biblioteca
{

    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas para navegación con parámetros
            Routing.RegisterRoute("bookdetail", typeof(BookDetailPage));
            Routing.RegisterRoute("addbook", typeof(AddBookPage));

            // Opcional: registrar las demás rutas por si se usan con GoToAsync
            Routing.RegisterRoute("library", typeof(LibraryPage));
            Routing.RegisterRoute("search", typeof(SearchPage));
            Routing.RegisterRoute("statistics", typeof(StatisticsPage));
        }
    }
}