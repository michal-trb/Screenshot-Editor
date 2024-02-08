using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace screenerWpf.Views
{
    public partial class ResizeDialog : Window
    {
        private int originalWidth;
        private int originalHeight;
        public int NewWidth { get; private set; }
        public int NewHeight { get; private set; }

        public ResizeDialog(int currentWidth, int currentHeight)
        {
            InitializeComponent();
            originalWidth = currentWidth;
            originalHeight = currentHeight;
            WidthTextBox.Text = currentWidth.ToString();
            HeightTextBox.Text = currentHeight.ToString();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValidWidth = int.TryParse(WidthTextBox.Text, out int width);
            bool isValidHeight = int.TryParse(HeightTextBox.Text, out int height);
            bool isValidPercentage = double.TryParse(PercentageTextBox.Text, out double percentage);

            if (isValidPercentage && percentage > 0)
            {
                NewWidth = (int)(width * (percentage / 100 + 1));
                NewHeight = (int)(height * (percentage / 100 + 1));
            }
            else if (isValidWidth && isValidHeight)
            {
                NewWidth = width;
                NewHeight = height;
            }
            else
            {
                MessageBox.Show("Please enter valid values.");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void PercentageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(PercentageTextBox.Text, out double percentage) && percentage > 0)
            {
                int newWidth = (int)(originalWidth * (percentage / 100.0));
                int newHeight = (int)(originalHeight * (percentage / 100.0));

                // Aktualizacja TextBoxów może nie być najlepszym rozwiązaniem UX
                // Rozważ użycie etykiet do pokazania przeliczonych nowych rozmiarów
                WidthTextBox.Text = newWidth.ToString();
                HeightTextBox.Text = newHeight.ToString();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
