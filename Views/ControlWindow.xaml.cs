using System;
using System.Windows;

namespace screenerWpf.Views
{
    public partial class ControlWindow : Window
    {
        public event EventHandler StopRequested;

        public ControlWindow()
        {
            InitializeComponent();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
            this.Close();
        }
    }
}
