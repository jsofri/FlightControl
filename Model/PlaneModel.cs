using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace PlaneController.Model
{
    class PlaneModel : IPlaneModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private volatile MessageQueue _queue;
        private TelnetClient _client;
        private string _ip = null;
        private string _port = null;

        // Thread of run loop.
        Thread t;

        private double _headingDeg;
        private double _gpsVerticalSpeed;
        private double _gpsGroundSpeed;
        private double _gpsAltitude;
        private double _pitoSpeed;
        private double _pitoAltitude;
        private double _roll;
        private double _pitch;
        private double _latitude;
        private double _longitude;


        public double HeadingDeg
        {
            get { return _headingDeg; }
            set
            {
                if (value != _headingDeg)
                {
                    _headingDeg = FixedValue(value, 0, 359.9999999);
                    NotifyPropertyChanged("HeadingDeg");
                }
            }
        }

        public double GPSVerticalSpeed
        {
            get { return _gpsVerticalSpeed; }
            set
            {
                if (value != _gpsVerticalSpeed)
                {

                    _gpsVerticalSpeed = FixedValue(value, -5000, 721); ;
                    NotifyPropertyChanged("GPSVerticalSpeed");
                }
            }
        }

        // Maximum 302 km/h, 163 knots.
        public double GPSGroundSpeed
        {
            get { return _gpsGroundSpeed; }
            set
            {
                if (value != _gpsGroundSpeed)
                {
                    _gpsGroundSpeed = FixedValue(value, -50, 302);
                    NotifyPropertyChanged("GPSGroundSpeed");
                }
            }
        }

        // Maximum 13500 feet
        public double GPSAltitude
        {
            get { return _gpsAltitude; }
            set
            {
                if (value != _gpsAltitude)
                {
                    _gpsAltitude = FixedValue(value, -1400, 13500);
                    NotifyPropertyChanged("GPSAltitude");
                }
            }
        }


        // Maximum 228 km/h, 123 knots.
        public double PitoSpeed
        {
            get { return _pitoSpeed; }
            set
            {
                if (value != _pitoSpeed)
                {
                    _pitoSpeed = FixedValue(value, 0, 228);
                    NotifyPropertyChanged("PitoSpeed");
                }
            }
        }

        // Maximum 13500 feet
        public double PitoAltitude
        {
            get { return _pitoAltitude; }
            set
            {
                if (value != _pitoAltitude)
                {
                    _pitoAltitude = FixedValue(value, -1400, 13500);
                    NotifyPropertyChanged("PitoAltitude");
                }
            }
        }


        public double Roll
        {
            get { return _roll; }
            set
            {
                if (value != _roll)
                {
                    _roll = FixedValue(value, 0, 359.999999);
                    NotifyPropertyChanged("Roll");
                }
            }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                if (value != _pitch)
                {
                    _pitch = FixedValue(value, 0, 359.999999);
                    NotifyPropertyChanged("Pitch");
                }
            }
        }

        // min -90 max 90
        public double Latitude
        {
            get { return _latitude; }
            set
            {
                if (value != _latitude)
                {
                    _latitude = FixedValue(value, -90, 90);
                    if (_latitude == value)
                    {
                        NotifyPropertyChanged("Latitude");
                    }
                    else
                    {
                        NotifyPropertyChanged("NIE");
                    }
                }
            }
        }

        // min -180 max 180
        public double Longitude
        {
            get { return _longitude; }
            set
            {
                if (value != _longitude)
                {
                    _longitude = FixedValue(value, -180, 180);
                    if (_longitude == value)
                    {
                        NotifyPropertyChanged("Longitude");
                    }
                    else
                    {
                        NotifyPropertyChanged("NIE");
                    }
                }
            }
        }


        public PlaneModel()
        {
            _queue = MessageQueue.GetInstance();
        }

        public void Connect(string ip, string port)
        {
            this.SetClient();

            if (_port == null || _ip == null)
            {
                _ip = ip;
                _port = port;
            }

            try
            {
                _client.Connect(ip, port);
                t = new Thread(new ThreadStart(_client.Run));
                t.Start();
            }
            catch (Exception e)
            {
                if (t.IsAlive)
                    Disconnect();
                throw new Exception("error in connecting to server");
            }
        }

        // Disconnect from Telnet client.
        public void Disconnect()
        {
            _client.Stop();
            t.Join();
        }

        /*
         * Set value of aileron using helper function.
         * Range of Aileron is [-1, 1].
         */
        public void SetAileron(double value)
        {
            GenericSetMessage(value, -1, 1, "flight/aileron ");
        }

        /*
         * Set value of Elevator using helper function.
         * Range of elevator is [-1, 1].
         */
        public void SetElevator(double value)
        {
            GenericSetMessage(value, -1, 1, "flight/elevator ");
        }

        /*
         * Set value of Rudder using helper function.
         * Range of rudder is [-1, 1].
         */
        public void SetRudder(double value)
        {
            GenericSetMessage(value, -1, 1, "flight/rudder ");
        }

        /*
         * Set value of throttle using helper function.
         * Range of throttle is [ 0 , 1 ].
         */
        public void SetThrottle(double value)
        {
            GenericSetMessage(value, 0, 1, "engines/current-engine/throttle ");
        }

        /*
         * Helper function for setting a value of a component in plane.
         * Check that value is in range and make an appropriate set command.
         */
        private void GenericSetMessage(double value, double min, double max,
                                      string suffix)
        {
            string message = "set /controls/" + suffix;

            value = FixedValue(value, min, max);

            message += (value.ToString() + '\n');

            _queue.Enqueue(message);
        }

        /*
         * Helper function to make sure the input "value" is in range.
         * If value is out of range [min, max], set it to be in the closer edge.
         * Return the closer value in the range.
         */
        private double FixedValue(double value, double min, double max)
        {
            if (value > max || value < min)
            {
                value = (value > max) ? max : min;
            }

            return value;
        }

        // Call PropertyChanged event with appropriate string of property.
        private void NotifyPropertyChanged(string message)
        {
            if (PropertyChanged != null && message.Length > 0)
            {
                if (!IsNotErrorMessage(message))
                {
                    // Write specific error and hande it.
                    message = HandleError(message); ;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs(message));
            }
        }

        /*
         * Get a string and check that it's not an error string.
         * Error strings are a convention between telnet client & this object.
         */
        private bool IsNotErrorMessage(string str)
        {
            bool boolean = true;

            switch (str)
            {
                case ("NaN"):
                case ("ERR"):
                case ("10 seconds"):
                case ("Socket close"):
                case ("NIE"):
                    boolean = false;
                    break;
                default:
                    break;
            }

            return boolean;
        }

        /*
         * Method to handle with errors in client/ server.
         * Operate accordingly to the problem.
         * Input string is a short description of the problem's type.
         */
        private string HandleError(string error)
        {
            string success, unsuccess;

            if (error == "NaN")
            {
                // server sent not a number
                return "Got a NaN from server";
            }
            else if (error == "ERR")
            {
                success = "Connection to server Error - reconnection succeeded!";
                unsuccess = "Connection to server Error - unable to reconnect";

                return TryReconnection(success, unsuccess);
            }
            else if (error == "10 seconds")
            {
                success = "TimeOut - Reconnected to server";
                unsuccess = "TimeOut - unable to reconnect to server";

                return TryReconnection(success, unsuccess);
            }
            else if (error == "NIE")
            {
                return "Server position is not in earth!";
            }

            return "Unknown error happaned";
        }

        /*
         * A helper method to try reconnection and returning mathing message.
         * Input strings are messages to return after trying to reconnect.
         */
        private string TryReconnection(string success, string unsuccess)
        {
            if (!Reconnect())
            {
                return unsuccess;
            }
            return success;
        }

        /*
         * Disconnect this object from current Telnet client.
         * Tries to reconnect for 5 seconds.
         * Return bool of success in reconnection.
         */
        private bool Reconnect()
        {
            bool status = false;
            Stopwatch sw = new Stopwatch();

            sw.Start();

            Disconnect();

            while (sw.ElapsedMilliseconds < 5000)
            {
                try
                {
                    Connect(_ip, _port);
                    status = true;
                }
                catch (Exception) { }
            }

            return status;
        }

        /*
         * Set components of the client - arrays of lambdas and commands.
         * Also set default error lambda function.
         */
        private void SetClient()
        {
            string[] messages = new string[8];
            Action<double>[] lambdas = new Action<double>[8];

            _client = new TelnetClient();

            messages[0] = "get /instrumentation/heading-indicator/indicated-heading-deg\n";
            lambdas[0] = (double value) => HeadingDeg = value;

            messages[1] = "get /instrumentation/gps/indicated-vertical-speed\n";
            lambdas[1] = (double value) => GPSVerticalSpeed = value;

            messages[2] = "get /instrumentation/gps/indicated-ground-speed-kt\n";
            lambdas[2] = (double value) => GPSGroundSpeed = value;

            messages[3] = "get /instrumentation/gps/indicated-altitude-ft\n";
            lambdas[3] = (double value) => GPSAltitude = value;

            messages[4] = "get /instrumentation/airspeed-indicator/indicated-speed-kt\n";
            lambdas[4] = (double value) => PitoSpeed = value;

            messages[5] = "get /instrumentation/altimeter/indicated-altitude-ft\n";
            lambdas[5] = (double value) => PitoAltitude = value;

            messages[6] = "get /instrumentation/attitude-indicator/internal-roll-deg\n";
            lambdas[6] = (double value) => Roll = value;

            messages[7] = "get /instrumentation/attitude-indicator/internal-pitch-deg\n";
            lambdas[7] = (double value) => Pitch = value;

            messages[8] = "get /position/latitude-deg\n";
            lambdas[8] = (double value) => Latitude = value;

            messages[9] = "get /position/longitude-deg\n";
            lambdas[9] = (double value) => Longitude = value;

            _client.SetRoutine(messages, lambdas);
            _client.SetDefaultErrorAction(NotifyPropertyChanged);
        }
    }
}
