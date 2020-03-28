using System.Windows;

namespace PlaneController
{
    // Class for main window - window that asks from user ip & port.
    // There are default values so user can just click start to run app.
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

        // Event handler for clicking on reset button.
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetData();
        }

        // Event handler for clicking on start button.
        // If ip and port are good - run a controller window.
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
                OpenErrorWindow();
            }
        }

        // Open a window of error in ip or port number.
        private void OpenErrorWindow()
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
                if (!IsLocalHost())
                {
                    System.Net.IPAddress.Parse(ip);
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

        private bool IsLocalHost()
        {
            if (this.ip == "localhost" || this.ip == "local host" || this.ip == "local_host")
            {
                ResetIP();
                return true;
            }

            return false;
        }
    }
}
