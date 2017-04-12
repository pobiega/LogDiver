using System;
using System.Windows;

namespace Logdiver
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FilterSettingsDialog : Window
    {
        public FilterSettingsDialog()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtFilter.SelectAll();
            txtFilter.Focus();
        }

        public string Filter => txtFilter.Text;

        private void BtnDialogOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
