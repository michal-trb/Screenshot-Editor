namespace screenerWpf.Views;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// A dialog window that allows the user to input new dimensions for resizing an image or element.
/// </summary>
public partial class ResizeDialog : Window
{
    private int originalWidth;
    private int originalHeight;

    /// <summary>
    /// Gets the new width entered by the user.
    /// </summary>
    public int NewWidth { get; private set; }

    /// <summary>
    /// Gets the new height entered by the user.
    /// </summary>
    public int NewHeight { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResizeDialog"/> class.
    /// Sets up the dialog with current dimensions of the element to be resized.
    /// </summary>
    /// <param name="currentWidth">The current width of the element.</param>
    /// <param name="currentHeight">The current height of the element.</param>
    public ResizeDialog(int currentWidth, int currentHeight)
    {
        InitializeComponent();
        originalWidth = currentWidth;
        originalHeight = currentHeight;
        WidthTextBox.Text = currentWidth.ToString();
        HeightTextBox.Text = currentHeight.ToString();
    }

    /// <summary>
    /// Minimizes the window when the minimize button is clicked.
    /// </summary>
    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Maximizes or restores the window when the maximize button is clicked.
    /// </summary>
    private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    /// <summary>
    /// Closes the window when the close button is clicked.
    /// </summary>
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Allows the user to drag the window using the mouse when clicking on the title bar.
    /// </summary>
    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    /// <summary>
    /// Confirms the resize operation and closes the dialog. 
    /// Uses either the new width and height or calculates new dimensions based on the percentage input.
    /// </summary>
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

    /// <summary>
    /// Updates the width and height fields when the percentage text box value changes.
    /// </summary>
    private void PercentageTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (double.TryParse(PercentageTextBox.Text, out double percentage) && percentage > 0)
        {
            int newWidth = (int)(originalWidth * (percentage / 100.0));
            int newHeight = (int)(originalHeight * (percentage / 100.0));

            WidthTextBox.Text = newWidth.ToString();
            HeightTextBox.Text = newHeight.ToString();
        }
    }

    /// <summary>
    /// Cancels the resize operation and closes the dialog.
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}
