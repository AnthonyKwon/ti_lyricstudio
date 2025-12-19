using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ti_Lyricstudio
{
    public class CustomGradientDrawOperation : ICustomDrawOperation
    {
        public Rect Bounds => _bounds;
        private readonly Rect _bounds;
        private readonly List<SKColor> _colors = [];
        private readonly float _time;

        public CustomGradientDrawOperation(Rect bounds, Color[] colors, float time)
        {
            _bounds = bounds;
            _time = time;

            // convert the avalonia color object to skiasharp color object
            foreach (Color color in colors)
                _colors.Add(new SKColor(color.R, color.G, color.B));
        }

        public void Dispose() {}
        public bool Equals(ICustomDrawOperation? other)
        {
            if (other != this) return false;
            else return true;
        }
        public bool HitTest(Point p) => false;

        // render the custom mesh gradient blur operation
        public void Render(ImmediateDrawingContext context)
        {
            // check if canvas is accessible
            ISkiaSharpApiLeaseFeature feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (feature == null) return;

            // get current canvas from lease
            using ISkiaSharpApiLease lease = feature.Lease();
            SKCanvas canvas = lease.SkCanvas;

            // speed and move distance for the points
            float speed = 0.02f;
            float distance = 300f;

            // get list of blending colors
            List<SKColor> blendedColors = [];
            for (int i = 0; i < 4; i++)
            {
                // find which two colors we are blending between
                float offset = i + (_time * speed * 0.05f);
                int indexA = (int)offset % 4;
                int indexB = (indexA + 1) % 4;
                float blend = offset - (int)offset;
                blendedColors.Add(LerpColor(_colors[indexA], _colors[indexB], blend));
            }
            blendedColors.Add(AverageColor(_colors));

            // position of each points
            SKPoint[] points =
            {
                new((float)Math.Sin(_time * speed) * distance,
                    (float)Math.Cos(_time * speed) * distance),
                new((float)(_bounds.Width + Math.Sin(_time * speed + 2) * distance),
                    (float)Math.Cos(_time * speed + 2) * distance),
                new((float)(_bounds.Width + Math.Sin(_time * speed + 4) * distance),
                    (float)(_bounds.Height + Math.Cos(_time * speed + 4) * distance)),
                new((float)Math.Sin(_time * speed + 5) * distance,
                    (float)(_bounds.Height + Math.Cos(_time * speed + 5) * distance)),
                new((float)((_bounds.Width / 2) + Math.Sin(_time * speed + 1.5) * (distance * 0.5f)),
                    (float)((_bounds.Height / 2) + Math.Cos(_time * speed + 1.5) * (distance * 0.5f)))
            };

            // 
            ushort[] indices = { 0, 1, 4, 1, 2, 4, 2, 3, 4, 3, 0, 4 };

            // 
            using SKPaint paint = new();
            //paint.ImageFilter = SKImageFilter.CreateBlur(25, 25);

            // draw gradient to the canvas
            SKVertices vertices = SKVertices.CreateCopy(SKVertexMode.Triangles, points, null, blendedColors.ToArray(), indices); 
            canvas.DrawVertices(vertices, SKBlendMode.SrcOver, paint);
        }

        // mix two colors smoothly
        private static SKColor LerpColor(SKColor c1, SKColor c2, float factor)
        {
            byte r = (byte)(c1.Red + (c2.Red - c1.Red) * factor);
            byte g = (byte)(c1.Green + (c2.Green - c1.Green) * factor);
            byte b = (byte)(c1.Blue + (c2.Blue - c1.Blue) * factor);
            return new SKColor(r, g, b);
        }

        // calculate average color for the center
        private static SKColor AverageColor(List<SKColor> colors)
        {
            int r = 0, g = 0, b = 0;
            for (int i = 0; i < 4; i++) {
                r += colors[i].Red;
                g += colors[i].Green;
                b += colors[i].Blue;
            }
            return new SKColor((byte)(r / 4), (byte)(g / 4), (byte)(b / 4));
        }
    }
}
