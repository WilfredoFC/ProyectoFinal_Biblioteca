using ProyectoFinal_Biblioteca.Services;

namespace ProyectoFinal_Biblioteca
{
    public partial class App : Application
    {
        public App(DatabaseService db)
        {
            InitializeComponent();

            Task.Run(async () => await db.SeedDataAsync());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}