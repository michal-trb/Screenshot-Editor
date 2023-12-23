using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace screenerWpf.Views
{
    /// <summary>
    /// Logika interakcji dla klasy OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
            screenshotsSavePath.Text = Properties.Settings.Default.ScreenshotsSavePath;
            screenshotsLibraryPath.Text = Properties.Settings.Default.ScreenshorsLibrary;
            recordsSavePath.Text = Properties.Settings.Default.RecordsSavePath;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ScreenshotsSavePath = screenshotsSavePath.Text;
            Properties.Settings.Default.ScreenshorsLibrary = screenshotsLibraryPath.Text;
            Properties.Settings.Default.RecordsSavePath = recordsSavePath.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
