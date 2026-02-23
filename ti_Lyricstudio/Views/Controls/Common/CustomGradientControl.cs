using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.IO;

namespace ti_Lyricstudio.Views.Controls
{
    public class CustomGradientControl : Control
    {
        public static readonly StyledProperty<Bitmap?> ArtworkProperty =
            AvaloniaProperty.Register<CustomGradientControl, Bitmap?>(nameof(Artwork));

        public static readonly StyledProperty<double> RenderScaleProperty =
            AvaloniaProperty.Register<CustomGradientControl, double>(nameof(RenderScale), defaultValue: 0.5);

        public Bitmap? Artwork
        {
            get => GetValue(ArtworkProperty);
            set => SetValue(ArtworkProperty, value);
        }

        /// <summary>
        /// Fraction of viewport resolution to render at (1.0 = full, 0.25 = quarter).
        /// Lower values improve performance at the cost of sharpness (invisible under heavy blur).
        /// Intended to be driven by a user setting.
        /// </summary>
        public double RenderScale
        {
            get => GetValue(RenderScaleProperty);
            set => SetValue(RenderScaleProperty, value);
        }

        static CustomGradientControl()
        {
            AffectsRender<CustomGradientControl>(ArtworkProperty);
        }

        // cached SKBitmap converted from the Avalonia Bitmap
        private SKBitmap? _skBitmap;

        private readonly DispatcherTimer _transitionTimer = new();
        private float elapsed = 0;

        public CustomGradientControl()
        {
            _transitionTimer.Interval = TimeSpan.FromTicks(416667);
            _transitionTimer.Tick += TransitionTimer_Tick;
            _transitionTimer.Start();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ArtworkProperty)
            {
                _skBitmap?.Dispose();
                _skBitmap = null;

                if (change.NewValue is Bitmap bmp)
                {
                    // Scale down to 128×128 using Avalonia first — encoding a tiny PNG is near-instant,
                    // avoiding the hundreds-of-ms stall from encoding a 3000×3000 source image
                    using Bitmap small = bmp.CreateScaledBitmap(new PixelSize(128, 128), BitmapInterpolationMode.LowQuality);
                    using var ms = new MemoryStream();
                    small.Save(ms);
                    ms.Position = 0;
                    _skBitmap = SKBitmap.Decode(ms);
                }
            }
        }

        private void TransitionTimer_Tick(object? sender, EventArgs e)
        {
            elapsed += 1f;
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            AlbumArtBackgroundDrawOperation operation =
                new(new Rect(0, 0, Bounds.Width, Bounds.Height), _skBitmap, elapsed, (float)RenderScale);

            context.Custom(operation);
        }
    }
}