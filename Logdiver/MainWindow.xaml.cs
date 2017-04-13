using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Logdiver.Filters;
using Logdiver.Util;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock.Layout;

namespace Logdiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpaceStation13ClientLog ss13;

        public MainWindow()
        {
            InitializeComponent();
        }

        public EventHandler<LineEventArgs> LineEventHandler;

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new FilterSettingsDialog();

            if (window.ShowDialog() != true) return;

            var filters = window.Filters;

            var txtBox = new FilteredTextBox
            {
                Filters = filters,
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Text = ss13?.GetMatchingContent(filters)
            };

            LineEventHandler += txtBox.OnLine;

            var item = new LayoutAnchorable
            {
                Content = txtBox,
                Title = filters.ToString(),
                CanAutoHide = false
            };

            item.Closing += (o, args) => LineEventHandler -= txtBox.OnLine;

            item.AddToLayout(DockingManager, AnchorableShowStrategy.Right);
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ss13 == null;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".htm",
                Filter = "SS13 log files|*.htm;*html"
            };

            if (ofd.ShowDialog() != true) return;
            ss13 = new SpaceStation13ClientLog(ofd.FileName, new DispatcherWinFormsCompatAdapter(Dispatcher));
            ss13.OnLine += OnLine;
            ss13.InitialRead();
        }

        /// <summary>
        /// Listens to the logreaders "line" event that sends a new line to distribute to all interested anchors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="lineEventArgs"></param>
        private void OnLine(object sender, LineEventArgs lineEventArgs)
        {
            LineEventHandler?.Invoke(sender, lineEventArgs);
        }

        private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ss13 != null;
        }

        private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ss13.Dispose();
            ss13 = null;
        }
    }
}