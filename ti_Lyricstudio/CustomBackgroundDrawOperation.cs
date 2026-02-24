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

        // max twist angle (radians) at the center of each copy
        private const float MaxTwistRad = 25f * MathF.PI / 180f;

        // SkSL radial twist shader (Skia M88 / SkiaSharp 2.88 syntax)
        private const string TwistSkSL = @"
            in fragmentProcessor image;
            uniform float2 center;
            uniform float  radius;
            uniform float  angle;

            half4 main(float2 coord) {
                float2 c = coord - center;
                float dist = length(c);
                if (dist < radius) {
                    float t = (radius - dist) / radius;
                    float a = t * t * angle;
                    float sinA = sin(a);
                    float cosA = cos(a);
                    c = float2(c.x * cosA - c.y * sinA, c.x * sinA + c.y * cosA);
                }
                return sample(image, c + center);
            }
        ";

        // compiled once on first use; null means compilation failed
        private static SKRuntimeEffect? _twistEffect;
        private static bool _twistEffectFailed;

        private static SKRuntimeEffect? GetTwistEffect()
        {
            if (_twistEffectFailed) return null;
            if (_twistEffect != null) return _twistEffect;

            _twistEffect = SKRuntimeEffect.Create(TwistSkSL, out _);
            if (_twistEffect == null) _twistEffectFailed = true;
            return _twistEffect;
        }

        /// <param name="renderScale">
        /// Fraction of viewport resolution to render at (1.0 = full, 0.25 = quarter).
        /// Blur sigma scales proportionally so the visual result stays consistent.
        /// Only applies to the GPU path; the CPU fallback always renders a flat color.
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

            GRContext? grContext = lease.GrContext;

            if (grContext != null)
                RenderGpu(canvas, grContext, w, h);
            else
                RenderCpuFallback(canvas, w, h);
        }

        private void RenderGpu(SKCanvas canvas, GRContext grContext, int w, int h)
        {
            // clamp to the visible clip rect so blur runs on the viewport area only
            SKRect clip = canvas.LocalClipBounds;
            int visW = Math.Min(w, (int)Math.Ceiling(clip.Right));
            int visH = Math.Min(h, (int)Math.Ceiling(clip.Bottom));

            int rw = Math.Max(1, (int)(visW * _renderScale));
            int rh = Math.Max(1, (int)(visH * _renderScale));
            float blurSigma = BlurSigma * _renderScale;

            using SKSurface? offscreen = SKSurface.Create(grContext, false, new SKImageInfo(rw, rh));
            if (offscreen == null) return;

            SKCanvas off = offscreen.Canvas;
            off.Clear(SKColors.Transparent);

            using SKPaint layerPaint = new() { ImageFilter = SKImageFilter.CreateBlur(blurSigma, blurSigma) };
            off.SaveLayer(layerPaint);

            float cx = rw * 0.5f;
            float cy = rh * 0.5f;
            float shortSide = Math.Min(rw, rh);

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

                DrawCopy(off, _artwork!, px, py, size, phase);
            }

            off.Restore();

            using SKImage snapshot = offscreen.Snapshot();
            if (_renderScale >= 1f)
                canvas.DrawImage(snapshot, 0, 0);
            else
                canvas.DrawImage(snapshot, new SKRect(0, 0, w, h));
        }

        private void RenderCpuFallback(SKCanvas canvas, int w, int h)
        {
            SKColor color = AverageColor(_artwork!);
            using SKPaint paint = new() { Color = color };
            canvas.DrawRect(0, 0, w, h, paint);
        }

        private static SKColor AverageColor(SKBitmap bitmap)
        {
            SKColor[] pixels = bitmap.Pixels;
            long r = 0, g = 0, b = 0;
            foreach (SKColor c in pixels) { r += c.Red; g += c.Green; b += c.Blue; }
            int n = pixels.Length;
            return new SKColor((byte)(r / n), (byte)(g / n), (byte)(b / n));
        }

        private static void DrawCopy(SKCanvas canvas, SKBitmap source,
            float centerX, float centerY, float size, float rotationTime)
        {
            int isize = Math.Max(1, (int)size);
            using SKBitmap scaled = source.Resize(new SKImageInfo(isize, isize), SKFilterQuality.Low);
            if (scaled == null) return;

            float left = centerX - size * 0.5f;
            float top  = centerY - size * 0.5f;

            float rotateDeg  = (float)Math.Sin(rotationTime * 0.6f) * 143f;
            float twistAngle = (float)Math.Sin(rotationTime * 0.6f) * MaxTwistRad;

            float s = Saturation;
            float[] mat =
            {
                0.213f + 0.787f * s,  0.715f - 0.715f * s,  0.072f - 0.072f * s,  0f, 0f,
                0.213f - 0.213f * s,  0.715f + 0.285f * s,  0.072f - 0.072f * s,  0f, 0f,
                0.213f - 0.213f * s,  0.715f - 0.715f * s,  0.072f + 0.928f * s,  0f, 0f,
                0f,                   0f,                    0f,                   1f, 0f
            };
            using SKColorFilter satFilter = SKColorFilter.CreateColorMatrix(mat);

            // local matrix maps canvas coords → bitmap pixel coords
            float scale = isize / size;
            SKMatrix localMatrix = SKMatrix.CreateScaleTranslation(scale, scale, -left * scale, -top * scale);
            using SKShader imageShader = scaled.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp, localMatrix);

            SKRuntimeEffect? effect = GetTwistEffect();
            if (effect != null)
            {
                var uniforms = new SKRuntimeEffectUniforms(effect);
                uniforms["center"] = new[] { centerX, centerY };
                uniforms["radius"] = size * 0.5f;
                uniforms["angle"]  = twistAngle;

                var children = new SKRuntimeEffectChildren(effect);
                children["image"] = imageShader;

                using SKShader shader = effect.ToShader(true, uniforms, children);
                using SKPaint paint = new() { Shader = shader, ColorFilter = satFilter };

                canvas.Save();
                canvas.RotateDegrees(rotateDeg, centerX, centerY);
                canvas.DrawRect(left, top, size, size, paint);
                canvas.Restore();
            }
        }
    }
}