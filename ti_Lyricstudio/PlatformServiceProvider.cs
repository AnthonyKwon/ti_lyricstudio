using Avalonia.Controls.ApplicationLifetimes;

namespace ti_Lyricstudio.Models
{
    public interface IPlatformServiceProvidor
    {
        public string GetClipboard();
        public void SetClipboard(string content);
    }

    public class PlatformServiceProvider : IPlatformServiceProvidor
    {
        private readonly IClassicDesktopStyleApplicationLifetime _desktop;

        public PlatformServiceProvider(IClassicDesktopStyleApplicationLifetime desktop)
        {
            _desktop = desktop;
        }

        public string GetClipboard()
        {
            // ignore request when clipboard is null
            if (_desktop.MainWindow == null ||
                _desktop.MainWindow.Clipboard == null) return string.Empty;

            return _desktop.MainWindow.Clipboard.GetTextAsync().GetAwaiter().GetResult() ?? string.Empty;
        }

        /// <summary>
        /// Set clipboard content to specified content.
        /// </summary>
        /// <param name="content">String content to set clipboard</param>
        public void SetClipboard(string content)
        {
            // ignore request when clipboard is null
            if (_desktop.MainWindow == null ||
                _desktop.MainWindow.Clipboard == null) return;

            // set clipboard to content
            _desktop.MainWindow.Clipboard.SetTextAsync(content);
        }
    }
}
