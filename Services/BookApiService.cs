using ProyectoFinal_Biblioteca.Models;
using System.Text.Json;

namespace ProyectoFinal_Biblioteca.Services
{
    public class BookApiService
    {
        private readonly HttpClient _httpClient;
        private const string SearchUrl = "https://openlibrary.org/search.json";
        private const string WorksUrl = "https://openlibrary.org/works";

        // Caché en memoria para resultados de búsqueda
        private static readonly Dictionary<string, List<BookSearchResult>> _searchCache = new();
        private static readonly Dictionary<string, BookDetail> _detailCache = new();

        public BookApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Busca libros por título o autor usando OpenLibrary API.
        /// </summary>
        public async Task<List<BookSearchResult>> SearchBooksAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<BookSearchResult>();

            var cacheKey = query.Trim().ToLowerInvariant();

            // Verificar caché
            lock (_searchCache)
            {
                if (_searchCache.TryGetValue(cacheKey, out var cachedResults))
                    return cachedResults;
            }

            try
            {
                var url = $"{SearchUrl}?title={Uri.EscapeDataString(query)}&limit=40";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var results = ParseSearchResults(json);

                    // Guardar en caché
                    lock (_searchCache)
                    {
                        _searchCache[cacheKey] = results;
                    }
                    return results;
                }
                else
                {
                    throw new HttpRequestException($"Error en la API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Otros errores (sin conexión, etc.)
                throw new Exception($"Error al buscar libros: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el detalle completo de un libro por su ID de OpenLibrary.
        /// </summary>
        public async Task<BookDetail?> GetBookDetailAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            // Verificar caché de detalles
            lock (_detailCache)
            {
                if (_detailCache.TryGetValue(id, out var cachedDetail))
                    return cachedDetail;
            }

            try
            {
                var url = $"{WorksUrl}/{id}.json";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var detail = ParseBookDetail(json);
                    if (detail != null)
                    {
                        lock (_detailCache)
                        {
                            _detailCache[id] = detail;
                        }
                    }
                    return detail;
                }
                else
                {
                    return null; // Libro no encontrado u otro error no crítico
                }
            }
            catch
            {
                return null;
            }
        }

        // ------------------------------------------------------------
        // Parseo de resultados (privados)
        // ------------------------------------------------------------

        private List<BookSearchResult> ParseSearchResults(string json)
        {
            var results = new List<BookSearchResult>();
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("docs", out var docs))
                    return results;

                foreach (var doc_item in docs.EnumerateArray())
                {
                    // Usar el primer identificador disponible (ebook_key, has_fulltext, etc.)
                    var id = "";
                    if (doc_item.TryGetProperty("key", out var keyProp))
                    {
                        id = keyProp.GetString()?.Split('/').LastOrDefault() ?? "";
                    }

                    if (string.IsNullOrEmpty(id))
                        continue;

                    var title = doc_item.TryGetProperty("title", out var t) 
                        ? t.GetString() ?? "Sin título" 
                        : "Sin título";

                    var author = "Autor desconocido";
                    if (doc_item.TryGetProperty("author_name", out var authors))
                    {
                        var authorList = authors.EnumerateArray()
                            .Select(x => x.GetString())
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Take(3)
                            .ToList();
                        if (authorList.Count > 0)
                            author = string.Join(", ", authorList);
                    }

                    var thumbnail = "";
                    if (doc_item.TryGetProperty("cover_i", out var coverId))
                    {
                        var coverIdValue = coverId.GetInt64();
                        thumbnail = $"https://covers.openlibrary.org/b/id/{coverIdValue}-M.jpg";
                    }

                    results.Add(new BookSearchResult
                    {
                        Id = id,
                        Title = title,
                        Author = author,
                        ThumbnailUrl = thumbnail
                    });
                }
            }
            catch
            {
                // En caso de error al parsear, retornar lista vacía
                return new List<BookSearchResult>();
            }

            return results;
        }

        private BookDetail? ParseBookDetail(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var title = root.TryGetProperty("title", out var t) 
                    ? t.GetString() ?? "Sin título" 
                    : "Sin título";

                var author = "Autor desconocido";
                if (root.TryGetProperty("authors", out var authorsArray))
                {
                    var authorList = new List<string>();
                    foreach (var auth in authorsArray.EnumerateArray())
                    {
                        if (auth.TryGetProperty("name", out var name))
                        {
                            authorList.Add(name.GetString() ?? "");
                        }
                    }
                    if (authorList.Count > 0)
                        author = string.Join(", ", authorList.Where(x => !string.IsNullOrEmpty(x)));
                }

                var isbn = "";
                if (root.TryGetProperty("isbn", out var isbnArray))
                {
                    isbn = isbnArray.EnumerateArray().FirstOrDefault().GetString() ?? "";
                }

                var publishedYear = 0;
                if (root.TryGetProperty("first_publish_year", out var year))
                {
                    publishedYear = year.GetInt32();
                }

                var pageCount = 0;
                if (root.TryGetProperty("number_of_pages", out var pages))
                {
                    pageCount = pages.GetInt32();
                }

                var description = "";
                if (root.TryGetProperty("description", out var desc))
                {
                    description = desc.ValueKind == JsonValueKind.String 
                        ? desc.GetString() ?? "" 
                        : "";
                }

                var coverUrl = "";
                if (root.TryGetProperty("covers", out var covers) && covers.GetArrayLength() > 0)
                {
                    var coverId = covers.EnumerateArray().First().GetInt64();
                    coverUrl = $"https://covers.openlibrary.org/b/id/{coverId}-M.jpg";
                }

                var categories = new List<string>();
                if (root.TryGetProperty("subjects", out var subjects))
                {
                    categories = subjects.EnumerateArray()
                        .Select(s => s.GetString() ?? "")
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Take(10)
                        .ToList();
                }

                return new BookDetail
                {
                    Title = title,
                    Author = author,
                    Isbn = isbn,
                    PublishedYear = publishedYear,
                    PageCount = pageCount,
                    Description = description,
                    CoverUrl = coverUrl,
                    Categories = categories
                };
            }
            catch
            {
                return null;
            }
        }
    }
}