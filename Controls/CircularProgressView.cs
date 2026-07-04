using Microsoft.Maui.Graphics;

namespace MetanetA_MobileApp.Controls;

public class CircularProgressView : GraphicsView, IDrawable
{
    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(
            nameof(Progress),
            typeof(double),
            typeof(CircularProgressView),
            0d,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty CenterTextProperty =
        BindableProperty.Create(
            nameof(CenterText),
            typeof(string),
            typeof(CircularProgressView),
            string.Empty,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty SubTextProperty =
        BindableProperty.Create(
            nameof(SubText),
            typeof(string),
            typeof(CircularProgressView),
            string.Empty,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(
            nameof(ProgressColor),
            typeof(Color),
            typeof(CircularProgressView),
            Color.FromArgb("#19C789"),
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty TrackColorProperty =
        BindableProperty.Create(
            nameof(TrackColor),
            typeof(Color),
            typeof(CircularProgressView),
            Color.FromArgb("#FFFFFF35"),
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(CircularProgressView),
            Colors.White,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty FillColorProperty =
        BindableProperty.Create(
            nameof(FillColor),
            typeof(Color),
            typeof(CircularProgressView),
            Color.FromArgb("#20FFFFFF"),
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty StrokeSizeProperty =
        BindableProperty.Create(
            nameof(StrokeSize),
            typeof(float),
            typeof(CircularProgressView),
            7f,
            propertyChanged: OnVisualPropertyChanged);

    public CircularProgressView()
    {
        Drawable = this;
    }

    /// <summary>
    /// 0-100 arası dəyər. Məsələn 88 verilsə, dairənin 88%-i progress rəngində çəkilir.
    /// </summary>
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public string CenterText
    {
        get => (string)GetValue(CenterTextProperty);
        set => SetValue(CenterTextProperty, value);
    }

    public string SubText
    {
        get => (string)GetValue(SubTextProperty);
        set => SetValue(SubTextProperty, value);
    }

    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Color FillColor
    {
        get => (Color)GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
    }

    public float StrokeSize
    {
        get => (float)GetValue(StrokeSizeProperty);
        set => SetValue(StrokeSizeProperty, value);
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var progress = Math.Clamp(Progress, 0, 100) / 100d;

        var size = Math.Min(dirtyRect.Width, dirtyRect.Height);
        var stroke = StrokeSize;

        var padding = stroke + 2;
        var x = dirtyRect.Center.X - size / 2 + padding;
        var y = dirtyRect.Center.Y - size / 2 + padding;
        var diameter = size - padding * 2;

        canvas.Antialias = true;
        canvas.StrokeSize = stroke;
        canvas.StrokeLineCap = LineCap.Round;

        // Track
        canvas.StrokeColor = TrackColor;
        canvas.DrawEllipse(x, y, diameter, diameter);

        // Progress
        if (progress > 0)
        {
            canvas.StrokeColor = ProgressColor;

            var startAngle = -90f;
            var endAngle = startAngle + (float)(360d * progress);

            canvas.DrawArc(
                x,
                y,
                diameter,
                diameter,
                startAngle,
                endAngle,
                clockwise: true,
                closed: false);
        }

        // Fill
        if (FillColor is not null)
        {
            canvas.FillColor = FillColor;
            canvas.FillEllipse(x + stroke, y + stroke, diameter - stroke * 2, diameter - stroke * 2);
        }

        canvas.FontColor = TextColor;

        // Kiçik dairələr üçün font avtomatik balacalaşır
        var centerFontSize = size <= 78 ? 19 : 22;
        var subFontSize = size <= 78 ? 8 : 10;

        // Center text üçün yuxarı hissə
        var centerTextRect = new RectF(
            dirtyRect.X,
            dirtyRect.Y + size * 0.25f,
            dirtyRect.Width,
            size * 0.30f);

        canvas.FontSize = centerFontSize;
        canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;

        canvas.DrawString(
            CenterText,
            centerTextRect,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);

        // SubText üçün aşağı hissə, amma dairənin içində
        if (!string.IsNullOrWhiteSpace(SubText))
        {
            var subTextRect = new RectF(
                dirtyRect.X,
                dirtyRect.Y + size * 0.52f,
                dirtyRect.Width,
                size * 0.20f);

            canvas.FontSize = subFontSize;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;

            canvas.DrawString(
                SubText,
                subTextRect,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }
    }

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularProgressView view)
            view.Invalidate();
    }
}
