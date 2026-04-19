namespace ProyectoFinal_Biblioteca.Drawables
{
    public class StatisticsDrawable : IDrawable
    {
        public int TotalBooks { get; set; }
        public int ReadBooks { get; set; }
        public int UnreadBooks { get; set; }
        public Dictionary<string, int> BooksByGenre { get; set; } = new();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            float centerX = dirtyRect.Width / 2;
            float centerY = 150;
            float radius = 100;

            // Gráfico circular (leídos vs pendientes)
            float readAngle = 360f * ReadBooks / TotalBooks;
            canvas.StrokeColor = Colors.Black;
            canvas.DrawEllipse(centerX - radius, centerY - radius, radius * 2, radius * 2);
            canvas.FillColor = Colors.Green;
            canvas.FillArc(centerX - radius, centerY - radius, radius * 2, radius * 2, 0, readAngle, true);
            canvas.FillColor = Colors.Red;
            canvas.FillArc(centerX - radius, centerY - radius, radius * 2, radius * 2, readAngle, 360 - readAngle, true);

            // Gráfico de barras por género
            float barWidth = 40;
            float startX = 50;
            float yBase = 400;
            int i = 0;
            foreach (var genre in BooksByGenre)
            {
                float barHeight = genre.Value * 20;
                canvas.FillColor = Colors.Blue;
                canvas.FillRectangle(startX + i * (barWidth + 10), yBase - barHeight, barWidth, barHeight);
                canvas.DrawString(genre.Key, startX + i * (barWidth + 10), yBase + 5, barWidth, 30, HorizontalAlignment.Center, VerticalAlignment.Top);
                i++;
            }

            // Números grandes
            canvas.FontSize = 24;
            canvas.DrawString($"Total: {TotalBooks}", 20, 550, HorizontalAlignment.Left);
            canvas.DrawString($"Leídos: {ReadBooks}", 20, 590, HorizontalAlignment.Left);
            canvas.DrawString($"Pendientes: {UnreadBooks}", 20, 630, HorizontalAlignment.Left);
        }
    }
}