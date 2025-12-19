using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace ti_Lyricstudio.Views.Controls
{
    public class CustomGradientControl : Control
    {
        //
        public static readonly StyledProperty<Color> GradientColor1Property =
        AvaloniaProperty.Register<CustomGradientControl, Color>(nameof(GradientColor1), Colors.White);

        public Color GradientColor1
        {
            get => GetValue(GradientColor1Property);
            set => SetValue(GradientColor1Property, value);
        }

        //
        public static readonly StyledProperty<Color> GradientColor2Property =
        AvaloniaProperty.Register<CustomGradientControl, Color>(nameof(GradientColor2), Colors.White);

        public Color GradientColor2
        {
            get => GetValue(GradientColor2Property);
            set => SetValue(GradientColor2Property, value);
        }

        //
        public static readonly StyledProperty<Color> GradientColor3Property =
        AvaloniaProperty.Register<CustomGradientControl, Color>(nameof(GradientColor3), Colors.White);

        public Color GradientColor3
        {
            get => GetValue(GradientColor3Property);
            set => SetValue(GradientColor3Property, value);
        }

        //
        public static readonly StyledProperty<Color> GradientColor4Property =
        AvaloniaProperty.Register<CustomGradientControl, Color>(nameof(GradientColor4), Colors.White);

        public Color GradientColor4
        {
            get => GetValue(GradientColor4Property);
            set => SetValue(GradientColor4Property, value);
        }

        static CustomGradientControl()
        {
            AffectsRender<CustomGradientControl>(GradientColor1Property);
            AffectsRender<CustomGradientControl>(GradientColor2Property);
            AffectsRender<CustomGradientControl>(GradientColor3Property);
            AffectsRender<CustomGradientControl>(GradientColor4Property);
        }

        //
        private readonly DispatcherTimer _transitionTimer = new();
        private float elapsed = 0;

        public CustomGradientControl()
        {
            _transitionTimer.Interval = TimeSpan.FromTicks(416667);
            _transitionTimer.Tick += TransitionTimer_Tick;
            _transitionTimer.Start();
        }

        private void TransitionTimer_Tick(object? sender, EventArgs e)
        {
            elapsed = elapsed + 1f;
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            Color[] colors =
            {
                GradientColor1,
                GradientColor2,
                GradientColor3,
                GradientColor4
            };

            // call gradient draw operation
            CustomGradientDrawOperation operation = new(new Rect(0, 0, Bounds.Width, Bounds.Height), colors, elapsed);

            //
            context.Custom(operation);
        }
    }
}
