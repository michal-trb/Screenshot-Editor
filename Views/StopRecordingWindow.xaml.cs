using System.Windows;

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
            ((App)Application.Current).MainViewModelService.StopRecording();
            Close();
        }
    }
}