using System.Windows;

namespace PlaneController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ip;
        private string port;

        public string IP
        {
            set => ip = value;
            get => ip;
        }
        public string Port
        {
            set => port = value;
            get => port;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ResetData();
        }

        // Reset ip and port to default values from application configurtion.
        // Use helper functions.
        public void ResetData()
        {
            this.ResetIP();
            this.ResetPort();
        }

        // Reset ip to default value and update appropriate text box in window.
        private void ResetIP()
        {
            this.ip = System.Configuration.ConfigurationManager.AppSettings["ip"];
            IP_TB.Text = IP;
        }

        // Reset to default port and update appropriate text box in window.
        private void ResetPort()
        {
            this.port = System.Configuration.ConfigurationManager.AppSettings["port"];
            Port_TB.Text = Port;
        }

        // 
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetData();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (validInput())
            {
                ControllerWindow controllerWindow =
                    new ControllerWindow(this.ip, this.port);

                this.Close();
                controllerWindow.Show();
            }
            else
            {
                openErrorWindow();
            }
        }

        private void openErrorWindow()
        {
            IvalidParametersWindow errorWindow = new IvalidParametersWindow();
            errorWindow.Show();
        }

        // Check if IP and Port are valid.
        private bool validInput()
        {
            bool boolean = true;
            if (this.port.Length != 4) return false;
            try
            {
                if (this.ip != "localhost" && this.ip != "local host" && this.ip != "local_host")
                {
                    System.Net.IPAddress.Parse(ip);
                }
                else
                {
                    this.ResetIP();
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(port, @"^\d+$"))
                {
                    boolean = false;
                }
            }
            catch (System.Exception)
            {

                boolean = false;
            }

            return boolean;
        }
    }
}
