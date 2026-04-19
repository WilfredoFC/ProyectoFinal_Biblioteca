using SQLite;

namespace ProyectoFinal_Biblioteca.Models
{

    [Table("Books")]
    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public int PageCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public int Rating { get; set; }  // 1-5
    }
}