using System;
using System.Windows;
using Logdiver.Filters;

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

            this.Filters = new FilterGroup();
            this.PropertyGrid.SelectedObject = Filters;
        }

        public FilterSettingsDialog(FilterGroup filters)
        {
            InitializeComponent();

            this.Filters = filters;
            this.PropertyGrid.SelectedObject = Filters;
        }

        public FilterGroup Filters;

        private void BtnDialogOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
