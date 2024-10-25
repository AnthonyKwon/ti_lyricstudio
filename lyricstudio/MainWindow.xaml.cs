using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // marker to check if file is opened
        private bool opened = false;
        // marker to check if file has modified
        private bool modified = false;

        // information of the lyrics workspace file
        private LyricsFile file;
        // list of the lyrics to be used at the GridView
        private List<LyricData> lyrics;
        LyricsDataSource dataSource;

        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new(ImportCommand, ExecutedImport, CanImportOrSave));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Player.OnClosing();
        }
    }
}