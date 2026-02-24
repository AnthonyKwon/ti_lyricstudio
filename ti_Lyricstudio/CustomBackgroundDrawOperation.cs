using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using System;

namespace ti_Lyricstudio
{
    public class CustomBackgroundDrawOperation : ICustomDrawOperation
    {
        public Rect Bounds => _bounds;
        private readonly Rect _bounds;
        private readonly SKBitmap? _artwork;
        private readonly float _time;
        private readonly float _renderScale;

        // speed at which the animation plays
        private const float Speed = 0.008f;

        // blur sigma at full resolution — scales proportionally with render scale
        private const float BlurSigma = 80f;

        // saturation multiplier (> 1 = oversaturated)
        private const float Saturation = 2.0f;

        // number of horizontal slices used to approximate the radial twist warp
        private const int TwistSlices = 8;

        // max additional twist angle (degrees) applied at the center of each copy
        private const float MaxTwistDeg = 25f;


        /// <param name="renderScale">
        /// Fraction of viewport resolution to render at (1.0 = full, 0.25 = quarter).
        /// Blur sigma scales proportionally so the visual result stays consistent.
        /// </param>
        public CustomBackgroundDrawOperation(Rect bounds, SKBitmap? artwork, float time, float renderScale = 1f)
        {
            _bounds = bounds;
            _artwork = artwork;
            _time = time;
            _renderScale = Math.Clamp(renderScale, 0.01f, 1f);
        }

        public void Dispose() { }

        public bool Equals(ICustomDrawOperation? other) => other == this;

        public bool HitTest(Point p) => false;

        public void Render(ImmediateDrawingContext context)
        {
            ISkiaSharpApiLeaseFeature? feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (feature == null) return;

            using ISkiaSharpApiLease lease = feature.Lease();
            SKCanvas canvas = lease.SkCanvas;

            int w = (int)_bounds.Width;
            int h = (int)_bounds.Height;
            if (w <= 0 || h <= 0) return;
            if (_artwork == null) return;

            int rw = Math.Max(1, (int)(w * _renderScale));
            int rh = Math.Max(1, (int)(h * _renderScale));
            // blur sigma scales with render scale so the visual result stays consistent
            float blurSigma = BlurSigma * _renderScale;

            using SKSurface offscreen = SKSurface.Create(new SKImageInfo(rw, rh));
            if (offscreen == null) return;

            SKCanvas off = offscreen.Canvas;
            off.Clear(SKColors.Transparent);

            // use SaveLayer with blur so the entire composite gets blurred in one pass
            using SKPaint layerPaint = new() { ImageFilter = SKImageFilter.CreateBlur(blurSigma, blurSigma) };
            off.SaveLayer(layerPaint);

            float cx = rw * 0.5f;
            float cy = rh * 0.5f;
            float shortSide = Math.Min(rw, rh);

            // draw from largest to smallest so smaller copies appear on top
            float[] scales      = { 1.25f, 0.80f, 0.50f, 0.25f };
            bool[]  orbits      = { false, false, true,  true  };
            float[] twistPhases = { 0f,    1.5f,  3.0f,  4.5f  };

            for (int i = 0; i < 4; i++)
            {
                float size = shortSide * scales[i];
                float phase = _time * Speed + twistPhases[i];

                float px = cx;
                float py = cy;
                if (orbits[i])
                {
                    float orbitRadius = shortSide * 0.15f;
                    px += (float)Math.Sin(phase * 0.7f) * orbitRadius;
                    py += (float)Math.Cos(phase * 0.5f) * orbitRadius;
                }

                DrawCopy(off, _artwork, px, py, size, phase);
            }

            off.Restore();

            using SKImage snapshot = offscreen.Snapshot();
            // scale up to full canvas size when rendering below full resolution; draw 1:1 otherwise
            if (_renderScale >= 1f)
                canvas.DrawImage(snapshot, 0, 0);
            else
                canvas.DrawImage(snapshot, new SKRect(0, 0, w, h));
        }

        private static void DrawCopy(SKCanvas canvas, SKBitmap source,
            float centerX, float centerY, float size, float rotationTime)
        {
            int isize = Math.Max(1, (int)size);
            using SKBitmap scaled = source.Resize(new SKImageInfo(isize, isize), SKFilterQuality.Low);
            if (scaled == null) return;

            float left = centerX - size * 0.5f;
            float top  = centerY - size * 0.5f;

            // overall rotation oscillates over time — different per copy via rotationTime phase offset
            float rotateDeg = (float)Math.Sin(rotationTime * 0.6f) * 143f;

            // twist intensity follows the same sin wave so it stays in phase with the rotation
            float twistSign = (float)Math.Sin(rotationTime * 0.6f);

            float s = Saturation;
            float[] mat =
            {
                0.213f + 0.787f * s,  0.715f - 0.715f * s,  0.072f - 0.072f * s,  0f, 0f,
                0.213f - 0.213f * s,  0.715f + 0.285f * s,  0.072f - 0.072f * s,  0f, 0f,
                0.213f - 0.213f * s,  0.715f - 0.715f * s,  0.072f + 0.928f * s,  0f, 0f,
                0f,                   0f,                    0f,                   1f, 0f
            };
            using SKColorFilter satFilter = SKColorFilter.CreateColorMatrix(mat);
            using SKPaint paint = new() { ColorFilter = satFilter, BlendMode = SKBlendMode.SrcOver };

            float sliceH = size / TwistSlices;

            canvas.Save();
            canvas.RotateDegrees(rotateDeg, centerX, centerY);

            // draw in horizontal slices, each additionally rotated by an amount proportional
            // to its proximity to the copy's center — approximates a radial twist warp
            for (int i = 0; i < TwistSlices; i++)
            {
                float sliceTop = top + i * sliceH;

                // normalized distance of this strip's center from the copy center (-1..1)
                float dy = ((sliceTop + sliceH * 0.5f) - centerY) / (size * 0.5f);
                float twistFrac = Math.Max(0f, 1f - Math.Abs(dy));
                float twistDeg = twistFrac * twistFrac * MaxTwistDeg * twistSign;

                canvas.Save();
                // clip to this horizontal strip; wide X so the twist rotation doesn't clip laterally
                canvas.ClipRect(new SKRect(left - size, sliceTop, left + size * 2f, sliceTop + sliceH + 1f));
                canvas.RotateDegrees(twistDeg, centerX, centerY);
                canvas.DrawBitmap(scaled, left, top, paint);
                canvas.Restore();
            }

            canvas.Restore();
        }
    }
}