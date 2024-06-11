using System.Windows;

namespace screenerWpf.Views
{
    public partial class StopRecordingWindow : Window
    {
        public StopRecordingWindow()
        {
            InitializeComponent();
            Top = SystemParameters.WorkArea.Height - Height - 10;
            Left = SystemParameters.WorkArea.Width - Width - 10;
            this.Opacity = 0.25;
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            // Wywołaj zatrzymanie nagrywania
            ((App)Application.Current).MainViewModelService.StopRecording();
            Close();
        }
    }
}