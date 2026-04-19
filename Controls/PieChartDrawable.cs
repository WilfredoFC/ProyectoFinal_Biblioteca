using Microsoft.Maui.Graphics;

namespace ProyectoFinal_Biblioteca.Controls;

public class PieChartDrawable : IDrawable
{
    public int ReadCount { get; set; }
    public int PendingCount { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float total = ReadCount + PendingCount;
        if (total == 0) return;

        float readAngle = (ReadCount / total) * 360f;
        float pendingAngle = 360f - readAngle;

        var center = new PointF(dirtyRect.Center.X, dirtyRect.Center.Y);
        float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 * 0.8f;

        // Sector Leídos (Verde)
        canvas.FillColor = Color.FromArgb("#4CAF50");
        canvas.FillArc(center.X - radius, center.Y - radius, radius * 2, radius * 2, 0, readAngle, true);

        // Sector Pendientes (Naranja)
        canvas.FillColor = Color.FromArgb("#FF9800");
        canvas.FillArc(center.X - radius, center.Y - radius, radius * 2, radius * 2, readAngle, pendingAngle, true);

        // Texto central
        canvas.FontColor = Colors.Black;
        canvas.FontSize = 14;
        canvas.DrawString($"{total} Total", center.X, center.Y, HorizontalAlignment.Center);
    }
}