using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.IO;

namespace ti_Lyricstudio.Views.Controls
{
    public class CustomBackgroundControl : Control
    {
        public static readonly StyledProperty<Bitmap?> ArtworkProperty =
            AvaloniaProperty.Register<CustomBackgroundControl, Bitmap?>(nameof(Artwork));

        public static readonly StyledProperty<double> RenderScaleProperty =
            AvaloniaProperty.Register<CustomBackgroundControl, double>(nameof(RenderScale), defaultValue: 0.5);
        
        public static readonly StyledProperty<double> OffsetProperty =
            AvaloniaProperty.Register<CustomBackgroundControl, double>(nameof(Offset), defaultValue: 0.5);

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
        
        public double Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        static CustomBackgroundControl()
        {
            AffectsRender<CustomBackgroundControl>(ArtworkProperty, RenderScaleProperty);
        }

        private SKBitmap? artwork;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if (Parent is Control p) p.PropertyChanged += Parent_PropertyChanged;
            InvalidateVisual();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            if (Parent is Control p) p.PropertyChanged -= Parent_PropertyChanged;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs newProperty)
        {
            base.OnPropertyChanged(newProperty);

            if (newProperty.Property == ArtworkProperty)
            {
                artwork?.Dispose();
                artwork = null;

                if (newProperty.NewValue is Bitmap newArtwork)
                {
                    // Scale down to 128×128 using Avalonia first — encoding a tiny PNG is near-instant,
                    // avoiding the hundreds-of-ms stall from encoding a 3000×3000 source image
                    using Bitmap scaled = newArtwork.CreateScaledBitmap(new PixelSize(64, 64), BitmapInterpolationMode.None);
                    using MemoryStream stream = new();
                    scaled.Save(stream);
                    stream.Position = 0;
                    artwork = SKBitmap.Decode(stream);
                }
            }
        }

        private void Parent_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == BoundsProperty && sender is Control p)
            {
                double side = Math.Max(p.Bounds.Width, p.Bounds.Height);
                if (!Math.Equals(Width, side)) Width = side;
                if (!Math.Equals(Height, side)) Height = side;
            }
        }

        public override void Render(DrawingContext context)
        {
            CustomBackgroundDrawOperation operation = new(
                new Rect(0, 0, Bounds.Width, Bounds.Height),
                artwork, (float)RenderScale, (float)Offset);
            context.Custom(operation);
        }
    }
}
