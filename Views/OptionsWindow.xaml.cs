using screenerWpf.ViewModels;
using System.Windows;

namespace screenerWpf.Views
{
    /// <summary>
    /// Logika interakcji dla klasy OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow(OptionsViewModel optionsViewModel)
        {
            InitializeComponent();
            DataContext = optionsViewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as OptionsViewModel;
            viewModel?.SaveSettings();
            this.Close();
        }
    }
}
