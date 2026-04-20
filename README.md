# Biblioteca Personal – Proyecto Final

**Aplicación móvil desarrollada con .NET MAUI** que permite gestionar tu colección de libros, buscar nuevos títulos en la API de Google Books y visualizar estadísticas de lectura con gráficos interactivos.

---

## Descripción del proyecto

Este proyecto integra todos los conceptos aprendidos durante el curso:

- Persistencia local con **SQLite**
- Consumo de **Google Books API**
- Arquitectura **MVVM** con `CommunityToolkit.Mvvm`
- Navegación multi-página con **Shell** y menú tipo *Flyout*
- Interfaz profesional con temas **claro/oscuro** (Light/Dark)
- Visualización de estadísticas con **GraphicsView** (gráfico circular y de barras)
- Operaciones **CRUD** completas sobre la biblioteca personal

---

##Funcionalidades implementadas

### Parte 1 – Base de datos SQLite (5 puntos)
- Modelo `Book` con atributos: título, autor, género, ISBN, año, páginas, descripción, URL de portada, estado de lectura, puntuación.
- `DatabaseService` con métodos:
  - `SaveBookAsync` (insert/update inteligente)
  - `GetBooksAsync`, `GetBooksByGenreAsync`, `GetReadBooksAsync`, `GetUnreadBooksAsync`
  - `DeleteBookAsync`, `UpdateReadStatusAsync`, `GetStatisticsAsync`

### Parte 2 – Consumo de API (4 puntos)
- Integración con **Google Books API** (búsqueda por título/autor).
- Modelos `BookSearchResult` y `BookDetail`.
- Servicio `BookApiService` con caché en memoria y sistema de reintentos (backoff exponencial) para manejar errores 429.
- Posibilidad de agregar un libro encontrado en la API a la biblioteca local.

### Parte 3 – Navegación multi-página (3 puntos)
- Shell con `FlyoutItem`:
  - Mi Biblioteca (`LibraryPage`)
  - Buscar Libros (`SearchPage`)
  - Estadísticas (`StatisticsPage`)
- Rutas registradas para navegación con parámetros (`bookdetail`, `addbook`).

### Parte 4 – Interfaz profesional (4 puntos)
- `CollectionView` con tarjetas (`Frame`) que muestran portada, título, autor, género y estado de lectura.
- Botones para marcar como leído/pendiente, eliminar y ver detalles.
- `EmptyView` personalizado cuando no hay libros.
- Filtros: Todos, Leídos, Pendientes y por género (implementado parcialmente en el ViewModel).
- Búsqueda local por título o autor.

### Parte 5 – Estilos y temas (3 puntos)
- Recursos de color para temas claro y oscuro.
- Estilos globales: `TitleStyle`, `SubtitleStyle`, `CaptionStyle`, `CardStyle`, `PrimaryButtonStyle`, `SecondaryButtonStyle`, `StandardEntryStyle`.
- Adaptación dinámica mediante `AppThemeBinding`.

### Parte 6 – MVVM completo (3 puntos)
- 5 ViewModels: `LibraryViewModel`, `SearchViewModel`, `AddBookViewModel`, `BookDetailViewModel`, `StatisticsViewModel`.
- Uso de `[ObservableProperty]`, `[RelayCommand]`, `ObservableCollection`.
- Métodos asíncronos para operaciones de BD y API.
- Propiedades calculadas y notificación de cambios.

### Parte 7 – GraphicsView Estadísticas (3 puntos)
- Clase `StatisticsDrawable` que implementa `IDrawable`.
- Gráfico circular (leídos vs pendientes).
- Gráfico de barras (libros por género).
- Números grandes con totales.

### Características adicionales (requeridas)
- Filtros y búsqueda local en `LibraryPage`.
- Validación de campos (título obligatorio, rating entre 0-5).
- Manejo de errores con `try-catch` y mensajes al usuario.
- Indicador de carga (`ActivityIndicator`) en búsquedas.
- Confirmación antes de eliminar (`DisplayAlert`).
- Datos de prueba (10+ libros) incluidos mediante `SeedDataAsync`.

---

## Instrucciones de ejecución

### Requisitos previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (o superior)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) con la carga de trabajo **.NET MAUI** (o VS Code con extensiones)
- Android emulator / dispositivo físico / Windows Machine

### Pasos para compilar y ejecutar

1. **Clonar o descargar el proyecto**
   ```bash
   git clone https://github.com/tuusuario/BibliotecaPersonal.git
   cd BibliotecaPersonal
