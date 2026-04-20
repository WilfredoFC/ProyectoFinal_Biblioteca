using ProyectoFinal_Biblioteca.Models;
using System.Text.Json;

namespace ProyectoFinal_Biblioteca.Services
{
    public class BookApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.googleapis.com/books/v1/volumes";

        // Caché en memoria para resultados de búsqueda
        private static readonly Dictionary<string, List<BookSearchResult>> _searchCache = new();
        private static readonly Dictionary<string, BookDetail> _detailCache = new();

        public BookApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Busca libros por título o autor, con caché y reintentos en caso de error 429.
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

            // Intentar la llamada con reintentos
            const int maxRetries = 3;
            int delayMs = 1000; // 1 segundo inicial

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var url = $"{BaseUrl}?q={Uri.EscapeDataString(query)}&maxResults=40&langRestrict=es";
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
                    else if ((int)response.StatusCode == 429) // Too Many Requests
                    {
                        if (attempt == maxRetries)
                            throw new HttpRequestException("Límite de solicitudes a la API excedido. Intenta más tarde.");

                        // Esperar antes de reintentar (backoff exponencial)
                        await Task.Delay(delayMs);
                        delayMs *= 2;
                    }
                    else
                    {
                        // Otro error HTTP
                        throw new HttpRequestException($"Error en la API: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("429"))
                {
                    if (attempt == maxRetries) throw;
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
                catch (Exception ex)
                {
                    // Otros errores (sin conexión, etc.)
                    throw new Exception($"Error al buscar libros: {ex.Message}", ex);
                }
            }

            return new List<BookSearchResult>();
        }

        /// <summary>
        /// Obtiene el detalle completo de un libro por su ID de Google Books, con caché y reintentos.
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

            const int maxRetries = 3;
            int delayMs = 1000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var url = $"{BaseUrl}/{id}";
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
                    else if ((int)response.StatusCode == 429)
                    {
                        if (attempt == maxRetries)
                            throw new HttpRequestException("Límite de solicitudes a la API excedido. Intenta más tarde.");
                        await Task.Delay(delayMs);
                        delayMs *= 2;
                    }
                    else
                    {
                        return null; // Libro no encontrado u otro error no crítico
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("429"))
                {
                    if (attempt == maxRetries) throw;
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        // ------------------------------------------------------------
        // Parseo de resultados (privados)
        // ------------------------------------------------------------

        private List<BookSearchResult> ParseSearchResults(string json)
        {
            var results = new List<BookSearchResult>();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("items", out var items))
                return results;

            foreach (var item in items.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString();
                var volume = item.GetProperty("volumeInfo");
                var title = volume.GetProperty("title").GetString();
                var authors = volume.TryGetProperty("authors", out var a)
                    ? string.Join(", ", a.EnumerateArray().Select(x => x.GetString()))
                    : "Autor desconocido";
                var thumbnail = volume.TryGetProperty("imageLinks", out var img) &&
                                img.TryGetProperty("smallThumbnail", out var thumb)
                    ? thumb.GetString()
                    : "";

                results.Add(new BookSearchResult
                {
                    Id = id ?? "",
                    Title = title ?? "Sin título",
                    Author = authors,
                    ThumbnailUrl = thumbnail ?? ""
                });
            }

            return results;
        }

        private BookDetail? ParseBookDetail(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("volumeInfo", out var volume))
                return null;

            var title = volume.GetProperty("title").GetString();
            var authors = volume.TryGetProperty("authors", out var a)
                ? string.Join(", ", a.EnumerateArray().Select(x => x.GetString()))
                : "";
            var isbn = "";
            if (volume.TryGetProperty("industryIdentifiers", out var ids))
            {
                foreach (var idObj in ids.EnumerateArray())
                {
                    var type = idObj.GetProperty("type").GetString();
                    if (type == "ISBN_13" || type == "ISBN_10")
                    {
                        isbn = idObj.GetProperty("identifier").GetString() ?? "";
                        break;
                    }
                }
            }
            var publishedYear = 0;
            if (volume.TryGetProperty("publishedDate", out var date))
            {
                var dateStr = date.GetString();
                if (!string.IsNullOrEmpty(dateStr) && dateStr.Length >= 4)
                    int.TryParse(dateStr.Substring(0, 4), out publishedYear);
            }
            var pageCount = volume.TryGetProperty("pageCount", out var pages) ? pages.GetInt32() : 0;
            var description = volume.TryGetProperty("description", out var desc) ? desc.GetString() : "";
            var coverUrl = volume.TryGetProperty("imageLinks", out var img) && img.TryGetProperty("thumbnail", out var thumb)
                ? thumb.GetString()
                : "";
            var categories = volume.TryGetProperty("categories", out var cats)
                ? cats.EnumerateArray().Select(c => c.GetString() ?? "").ToList()
                : new List<string>();

            return new BookDetail
            {
                Title = title ?? "Sin título",
                Author = authors,
                Isbn = isbn,
                PublishedYear = publishedYear,
                PageCount = pageCount,
                Description = description ?? "",
                CoverUrl = coverUrl ?? "",
                Categories = categories
            };
        }
    }
}