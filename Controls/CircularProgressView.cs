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
        var progress = Math.Clamp(Progress, 0d, 100d) / 100d;
        var size = Math.Min(dirtyRect.Width, dirtyRect.Height);
        var stroke = StrokeSize;
        var padding = stroke + 2f;
        var diameter = size - padding * 2f;
        var x = dirtyRect.Center.X - diameter / 2f;
        var y = dirtyRect.Center.Y - diameter / 2f;

        canvas.Antialias = true;

        canvas.FillColor = FillColor;
        canvas.FillEllipse(x, y, diameter, diameter);

        canvas.StrokeSize = stroke;
        canvas.StrokeLineCap = LineCap.Round;

        canvas.StrokeColor = TrackColor;
        canvas.DrawEllipse(x, y, diameter, diameter);

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

        canvas.FontColor = TextColor;
        canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
        canvas.FontSize = 25;
        canvas.DrawString(
            CenterText,
            dirtyRect,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);

        if (!string.IsNullOrWhiteSpace(SubText))
        {
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.FontSize = 11;
            var subRect = new RectF(
                dirtyRect.X,
                dirtyRect.Center.Y + 16,
                dirtyRect.Width,
                18);

            canvas.DrawString(
                SubText,
                subRect,
                HorizontalAlignment.Center,
                VerticalAlignment.Top);
        }
    }

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularProgressView view)
            view.Invalidate();
    }
}
