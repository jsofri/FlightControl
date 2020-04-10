using System.Windows;

namespace FlightSimulator
{
    /*
     * Class for showing errors to user in a new window.
     * 
     * author: Jhonny.
     * date: 4.1.20
     */
    public partial class ErrorWindow : Window
    {

        // Input string will be the text in the window.
        public ErrorWindow(string error)
        {
            InitializeComponent();
            DataContext = this;
            Message.Content = error;
        }

        // Event handler.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
