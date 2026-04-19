using ProyectoFinal_Biblioteca.Models;
using System.Text.Json;

namespace ProyectoFinal_Biblioteca.Services
{
    public class BookApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.googleapis.com/books/v1/volumes";

        public BookApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<BookSearchResult>> SearchBooksAsync(string query)
        {
            var url = $"{BaseUrl}?q={Uri.EscapeDataString(query)}&maxResults=40";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<BookSearchResult>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var items = root.GetProperty("items");

            var results = new List<BookSearchResult>();
            foreach (var item in items.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString();
                var volume = item.GetProperty("volumeInfo");
                var title = volume.GetProperty("title").GetString();
                var authors = volume.TryGetProperty("authors", out var a) ? a.EnumerateArray().First().GetString() : "Unknown";
                var thumbnail = volume.TryGetProperty("imageLinks", out var img) && img.TryGetProperty("smallThumbnail", out var thumb) ? thumb.GetString() : "";

                results.Add(new BookSearchResult
                {
                    Id = id!,
                    Title = title!,
                    Author = authors!,
                    ThumbnailUrl = thumbnail ?? ""
                });
            }
            return results;
        }

        public async Task<BookDetail?> GetBookDetailAsync(string id)
        {
            var url = $"{BaseUrl}/{id}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var volume = doc.RootElement.GetProperty("volumeInfo");

            var title = volume.GetProperty("title").GetString();
            var authors = volume.TryGetProperty("authors", out var a) ? string.Join(", ", a.EnumerateArray().Select(x => x.GetString())) : "";
            var isbn = "";
            if (volume.TryGetProperty("industryIdentifiers", out var ids))
            {
                foreach (var idObj in ids.EnumerateArray())
                    if (idObj.GetProperty("type").GetString() == "ISBN_13")
                        isbn = idObj.GetProperty("identifier").GetString();
            }
            var publishedYear = volume.TryGetProperty("publishedDate", out var date) && date.GetString()!.Length >= 4 ? int.Parse(date.GetString()!.Substring(0, 4)) : 0;
            var pageCount = volume.TryGetProperty("pageCount", out var pages) ? pages.GetInt32() : 0;
            var description = volume.TryGetProperty("description", out var desc) ? desc.GetString() : "";
            var coverUrl = volume.TryGetProperty("imageLinks", out var img) && img.TryGetProperty("thumbnail", out var thumb) ? thumb.GetString() : "";
            var categories = volume.TryGetProperty("categories", out var cats) ? cats.EnumerateArray().Select(c => c.GetString()!).ToList() : new List<string>();

            return new BookDetail
            {
                Title = title!,
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