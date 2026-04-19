using Microsoft.Maui.Graphics;

namespace ProyectoFinal_Biblioteca.Controls;

public class BarChartDrawable : IDrawable
{
    public Dictionary<string, int> Data { get; set; } = new();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Data == null || Data.Count == 0) return;

        var sortedData = Data.OrderByDescending(x => x.Value).Take(5).ToList(); // Top 5 géneros
        float maxValue = sortedData.Max(x => x.Value);

        float barWidth = dirtyRect.Width / sortedData.Count;
        float chartHeight = dirtyRect.Height * 0.8f; // Espacio para etiquetas

        for (int i = 0; i < sortedData.Count; i++)
        {
            var item = sortedData[i];
            float barHeight = (item.Value / maxValue) * chartHeight;

            float x = i * barWidth;
            float y = dirtyRect.Height - barHeight - 20; // Margen inferior

            // Dibujar barra
            canvas.FillColor = Color.FromArgb("#512BD4");
            canvas.FillRectangle(x + 5, y, barWidth - 10, barHeight);

            // Etiqueta (Género)
            canvas.FontColor = Colors.Gray;
            canvas.FontSize = 10;
            canvas.DrawString(item.Key, x + barWidth / 2, dirtyRect.Height - 5, HorizontalAlignment.Center);

            // Valor
            canvas.DrawString(item.Value.ToString(), x + barWidth / 2, y - 5, HorizontalAlignment.Center);
        }
    }
}