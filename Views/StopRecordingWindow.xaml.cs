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
    /// Logika interakcji dla klasy StopRecordingWindow.xaml
    /// </summary>
    public partial class StopRecordingWindow : Window
    {
        public StopRecordingWindow()
        {
            InitializeComponent();
            Top = SystemParameters.WorkArea.Height - Height - 10;
            Left = SystemParameters.WorkArea.Width - Width - 10;
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            // Wywołaj zatrzymanie nagrywania
            ((App)Application.Current).MainViewModel.StopRecording();
            Close();
        }
    }
}