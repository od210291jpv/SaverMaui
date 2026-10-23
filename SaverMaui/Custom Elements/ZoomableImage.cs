using FFImageLoading.Maui;

namespace SaverMaui.Custom_Elements;

/// <summary>
/// A reusable ContentView that wraps an FFImageLoading CachedImage with pinch-to-zoom
/// and pan-when-zoomed functionality.
/// Scale is clamped between 1x and 4x. Double-tap resets zoom.
/// Uses the Microsoft-recommended incremental scale accumulation pattern.
/// </summary>
public class ZoomableImage : ContentView
{
    private const double MinScale = 1.0;
    private const double MaxScale = 4.0;
    private const uint ResetAnimationDuration = 250;

    private double _currentScale = 1.0;
    private double _startScale = 1.0;
    private double _xOffset = 0;
    private double _yOffset = 0;

    private readonly CachedImage _image;

    #region Bindable Properties

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(
            nameof(ImageSource),
            typeof(ImageSource),
            typeof(ZoomableImage),
            default(ImageSource),
            propertyChanged: OnImageSourceChanged);

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableImage zoomable && newValue is ImageSource source)
        {
            zoomable._image.Source = source;
        }
    }

    public static readonly BindableProperty ImageAspectProperty =
        BindableProperty.Create(
            nameof(ImageAspect),
            typeof(Aspect),
            typeof(ZoomableImage),
            Aspect.AspectFit,
            propertyChanged: OnImageAspectChanged);

    public Aspect ImageAspect
    {
        get => (Aspect)GetValue(ImageAspectProperty);
        set => SetValue(ImageAspectProperty, value);
    }

    private static void OnImageAspectChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableImage zoomable)
        {
            zoomable._image.Aspect = (Aspect)newValue;
        }
    }

    public static readonly BindableProperty LoadingPlaceholderProperty =
        BindableProperty.Create(
            nameof(LoadingPlaceholder),
            typeof(string),
            typeof(ZoomableImage),
            default(string),
            propertyChanged: OnLoadingPlaceholderChanged);

    public string LoadingPlaceholder
    {
        get => (string)GetValue(LoadingPlaceholderProperty);
        set => SetValue(LoadingPlaceholderProperty, value);
    }

    private static void OnLoadingPlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableImage zoomable && newValue is string placeholder)
        {
            zoomable._image.LoadingPlaceholder = placeholder;
        }
    }

    public static readonly BindableProperty ErrorPlaceholderProperty =
        BindableProperty.Create(
            nameof(ErrorPlaceholder),
            typeof(string),
            typeof(ZoomableImage),
            default(string),
            propertyChanged: OnErrorPlaceholderChanged);

    public string ErrorPlaceholder
    {
        get => (string)GetValue(ErrorPlaceholderProperty);
        set => SetValue(ErrorPlaceholderProperty, value);
    }

    private static void OnErrorPlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableImage zoomable && newValue is string placeholder)
        {
            zoomable._image.ErrorPlaceholder = placeholder;
        }
    }

    public static readonly BindableProperty FadeAnimationEnabledProperty =
        BindableProperty.Create(
            nameof(FadeAnimationEnabled),
            typeof(bool),
            typeof(ZoomableImage),
            true,
            propertyChanged: OnFadeAnimationEnabledChanged);

    public bool FadeAnimationEnabled
    {
        get => (bool)GetValue(FadeAnimationEnabledProperty);
        set => SetValue(FadeAnimationEnabledProperty, value);
    }

    private static void OnFadeAnimationEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableImage zoomable)
        {
            zoomable._image.FadeAnimationEnabled = (bool)newValue;
        }
    }

    public static readonly BindableProperty FadeAnimationDurationProperty =
        BindableProperty.Create(
            nameof(FadeAnimationDuration),
            typeof(int),
            typeof(ZoomableImage),
            300,
            propertyChanged: OnFadeAnimationDurationChanged);

    public int FadeAnimationDuration
    {
        get => (int)GetValue(FadeAnimationDurationProperty);
        set => SetValue(FadeAnimationDurationProperty, value);
    }

    private static void OnFadeAnimationDurationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableImage zoomable)
        {
            zoomable._image.FadeAnimationDuration = (int)newValue;
        }
    }

    /// <summary>
    /// Indicates whether the image is currently zoomed beyond 1x.
    /// </summary>
    public bool IsZoomed => _currentScale > 1.05;

    #endregion

    /// <summary>
    /// Event raised on double-tap so the page can handle its existing logic.
    /// </summary>
    public event EventHandler DoubleTapped;

    public ZoomableImage()
    {
        _image = new CachedImage
        {
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            AnchorX = 0,
            AnchorY = 0,
        };

        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;

        var doubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        doubleTapGesture.Tapped += OnDoubleTapped;

        GestureRecognizers.Add(pinchGesture);
        GestureRecognizers.Add(panGesture);
        GestureRecognizers.Add(doubleTapGesture);

        Content = _image;
    }

    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                _startScale = _currentScale;
                break;

            case GestureStatus.Running:
                // e.Scale is INCREMENTAL (relative to previous event), not cumulative.
                // Use the Microsoft-recommended accumulation pattern:
                //   currentScale += (e.Scale - 1) * startScale
                _currentScale += (e.Scale - 1) * _startScale;
                _currentScale = Math.Clamp(_currentScale, MinScale, MaxScale);

                // Calculate the rendered position of the image, accounting for current offset
                double renderedX = _image.X + _xOffset;
                double renderedY = _image.Y + _yOffset;

                // Normalize against the container
                double deltaX = renderedX / Width;
                double deltaY = renderedY / Height;

                // Scale origin relative to actual image dimensions
                double deltaWidth = Width / (_image.Width * _startScale);
                double deltaHeight = Height / (_image.Height * _startScale);

                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate target translation to keep zoom centered on the pinch point
                double targetX = _xOffset - (originX * _image.Width) * (_currentScale - _startScale);
                double targetY = _yOffset - (originY * _image.Height) * (_currentScale - _startScale);

                // Clamp translation: with AnchorX/Y=0, image grows right/down,
                // so valid translation range is [-Width*(scale-1), 0]
                double maxTransX = _image.Width * (_currentScale - 1);
                double maxTransY = _image.Height * (_currentScale - 1);

                _image.TranslationX = Math.Clamp(targetX, -maxTransX, 0);
                _image.TranslationY = Math.Clamp(targetY, -maxTransY, 0);
                _image.Scale = _currentScale;
                break;

            case GestureStatus.Completed:
                // Persist current translation as offset for next gesture
                _xOffset = _image.TranslationX;
                _yOffset = _image.TranslationY;

                // Only auto-reset if basically not zoomed at all (accidental micro-pinch)
                if (_currentScale <= 1.05)
                {
                    ResetZoom();
                }
                break;
        }
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        // Only allow panning when zoomed in
        if (_currentScale <= 1.05)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Running:
                double newX = _xOffset + e.TotalX;
                double newY = _yOffset + e.TotalY;

                // With AnchorX/Y=0, valid range is [-Width*(scale-1), 0]
                double maxTransX = _image.Width * (_currentScale - 1);
                double maxTransY = _image.Height * (_currentScale - 1);

                _image.TranslationX = Math.Clamp(newX, -maxTransX, 0);
                _image.TranslationY = Math.Clamp(newY, -maxTransY, 0);
                break;

            case GestureStatus.Completed:
                _xOffset = _image.TranslationX;
                _yOffset = _image.TranslationY;
                break;
        }
    }

    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        // If zoomed, reset on double-tap; otherwise bubble the event up
        if (IsZoomed)
        {
            ResetZoom();
        }
        else
        {
            DoubleTapped?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Resets the zoom and pan to the default state with a smooth animation.
    /// Call this when the carousel swipes to a new item.
    /// </summary>
    public async void ResetZoom()
    {
        _currentScale = 1.0;
        _startScale = 1.0;
        _xOffset = 0;
        _yOffset = 0;

        await Task.WhenAll(
            _image.ScaleTo(1.0, ResetAnimationDuration, Easing.CubicOut),
            _image.TranslateTo(0, 0, ResetAnimationDuration, Easing.CubicOut)
        );

        // Ensure exact values after animation
        _image.Scale = 1.0;
        _image.TranslationX = 0;
        _image.TranslationY = 0;
    }
}
