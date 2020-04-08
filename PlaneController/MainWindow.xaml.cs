using PlaneController.Model;
using PlaneController.ViewModel;
using System;
using System.Windows;

namespace PlaneController
{
    /*
     * Class for main window - window that asks from user ip & port.
     * There are default values so user can just click start to run app.
     * 
     * author: Jhonny.
     * date: 4.1.20
     */
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

        /*
         * Reset ip and port to default values from application configurtion.
         * Use helper functions.
         */
        public void ResetData()
        {
            this.ResetIP();
            this.ResetPort();
        }

        // Reset ip to default value and update appropriate text box in window.
        private void ResetIP()
        {
            IP = System.Configuration.ConfigurationManager.AppSettings["ip"];
            IP_TB.Text = IP;
        }

        // Reset to default port and update appropriate text box in window.
        private void ResetPort()
        {
            Port = System.Configuration.ConfigurationManager.AppSettings["port"];
            Port_TB.Text = Port;
        }

        // Event handler for clicking on reset button.
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetData();
        }

        /*
         * Event handler for clicking on start button.
         * If ip and port are good - run a controller window.
         */
        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidInput())
            {
                PlaneModel model = new PlaneModel();

                if (Connect(model))
                {
                    ControllerWindow controllerWindow =
                    new ControllerWindow(new PlaneViewModel(model));

                    this.Close();
                    controllerWindow.Show();
                }
            }
            else
            {
                OpenErrorWindow("Invalid IP/ Port");
            }
        }

        // Open a window of error in ip or port number.
        private void OpenErrorWindow(string errorMessage)
        {
            ErrorWindow errorWindow = new ErrorWindow(errorMessage);
            errorWindow.Show();
        }

        // Check if IP and Port are valid.
        private bool ValidInput()
        {
            bool boolean = true;
            if (Port.Length != 4) return false;
            try
            {
                if (!IsLocalHost())
                {
                    System.Net.IPAddress.Parse(IP);
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

        // Check if ip member equals to some variation of 'local host'.
        private bool IsLocalHost()
        {
            if (IP == "localhost" || IP == "local host" || IP == "local_host")
            {
                ResetIP();
                return true;
            }

            return false;
        }

        // Tries to connect to the client. returns true if successed.
        private bool Connect(IPlaneModel model)
        {

            try
            {
                model.Connect(IP, Port);
                return true;
            }
            catch (Exception)
            {
            }

            OpenErrorWindow("Unable to connect");
            return false;
        }
    }
}
