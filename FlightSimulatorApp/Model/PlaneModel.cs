using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace FlightSimulator.Model
{

    /*
     * Object that implements IPlaneModel as a model in MVVM.
     * Use a Telnet client to control plane server.
     * when operated, executes a loop of messages and matching lambda expression.
     * Use MessageQueue to send set messages to Telnet client.
     * 
     * author: Jhonny.
     * date: 3.28.20
     */
    class PlaneModel : IPlaneModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private volatile MessageQueue _queue;
        private TelnetClient _client;
        private string _ip = null;
        private string _port = null;
        private const double EPSILON = 0.00001;
        private const double MIN_DIFF = 0.005;

        // Thread of run loop.
        Thread _threadOfLoop;

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
        private double _rudder = 0;
        private double _elevator = 0;


        public double HeadingDeg
        {
            get { return _headingDeg; }
            set
            {
                if (value != _headingDeg)
                {
                    _headingDeg = value;
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
                    _gpsVerticalSpeed = value;
                    NotifyPropertyChanged("GPSVerticalSpeed");
                }
            }
        }

        public double GPSGroundSpeed
        {
            get { return _gpsGroundSpeed; }
            set
            {
                if (value != _gpsGroundSpeed)
                {
                    _gpsGroundSpeed = value;
                    NotifyPropertyChanged("GPSGroundSpeed");
                }
            }
        }

        public double GPSAltitude
        {
            get { return _gpsAltitude; }
            set
            {
                if (value != _gpsAltitude)
                {
                    _gpsAltitude = value;
                    NotifyPropertyChanged("GPSAltitude");
                }
            }
        }

        public double PitoSpeed
        {
            get { return _pitoSpeed; }
            set
            {
                if (value != _pitoSpeed)
                {
                    _pitoSpeed = value;
                    NotifyPropertyChanged("PitoSpeed");
                }
            }
        }

        public double PitoAltitude
        {
            get { return _pitoAltitude; }
            set
            {
                if (value != _pitoAltitude)
                {
                    _pitoAltitude = value;
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
                    _roll = value;
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
                    _pitch = value;
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

                    if (Math.Abs(_latitude - value) <= EPSILON)
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

                    if (Math.Abs(_longitude - value) <= EPSILON)
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

        public List<string> Errors
        {
            get
            {
                return ErrorsQueue.Get();
            }
        }


        public PlaneModel()
        {
            _queue = MessageQueue.GetInstance();
        }

        // Connect to telnet client in given params and run it in other thread.
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
                _threadOfLoop = new Thread(new ThreadStart(_client.Run));
                _threadOfLoop.Start();
            }
            catch (Exception)
            {
                if (_threadOfLoop != null && _threadOfLoop.IsAlive)
                    Disconnect();
                throw new Exception("error in connecting to server");
            }
        }

        // Disconnect from Telnet client.
        public void Disconnect()
        {
            _client.Stop();
            //_threadOfLoop.Join();
        }

        /*
         * Set value of aileron using helper function.
         * Range of Aileron is [-1, 1].
         */
        public void SetAileron(double value)
        {
            GenericSetMessage(value, -1, 1, "controls/flight/aileron");
        }

        /*
         * Set value of Elevator using helper function.
         * Range of elevator is [-1, 1].
         */
        public void SetElevator(double value)
        {
            if (Math.Abs(_elevator - value) > MIN_DIFF)
            {
                GenericSetMessage(value, -1, 1, "controls/flight/elevator");
                _elevator = value;
            }
        }

        /*
         * Set value of Rudder using helper function.
         * Range of rudder is [-1, 1].
         */
        public void SetRudder(double value)
        {
            if (Math.Abs(_rudder - value) > MIN_DIFF)
            {
                GenericSetMessage(value, -1, 1, "controls/flight/rudder");
                _rudder = value;
            }
        }

        /*
         * Set value of throttle using helper function.
         * Range of throttle is [ 0 , 1 ].
         */
        public void SetThrottle(double value)
        {
            GenericSetMessage(value, 0, 1, "controls/engines/current-engine/throttle");
        }

        /*
         * Helper function for setting a value of a component in plane.
         * Check that value is in range and make an appropriate set command.
         */
        private void GenericSetMessage(double value, double min, double max,
                                        string suffix)
        {
            string message = "set /" + suffix + " ";

            value = FixedValue(value, min, max);

            message += (value.ToString() + "\n");

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
                if (IsErrorMessage(message))
                {
                    // Write specific error to error list and handle it.
                    HandleError(message);
                    message = "Errors";
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs(message));
            }
        }

        /*
         * Get a string and check that it's not an error string.
         * Error strings are a convention between telnet client & this object.
         */
        private bool IsErrorMessage(string str)
        {
            bool boolean = false;

            switch (str)
            {
                case ("Unknown"):
                case ("CNC"):
                case ("ERR"):
                case ("NaN"):
                case ("10 seconds"):
                case ("Socket close"):
                case ("NIE"):
                    boolean = true;
                    break;
                default:
                    break;
            }

            return boolean;
        }

        /*
         * Method to handle errors in client/ server.
         * Operate accordingly to the problem.
         * Input string is a short description of the problem's type.
         */
        private void HandleError(string error)
        {
            string message, time;

            // Postfix of current Hour
            time = " at " + DateTime.Now.ToString("T", CultureInfo.CreateSpecificCulture("de-DE"));

            if (error == "Socket close")
            {
                message = "Plane socket is closed";
            }
            else if (error == "10 seconds")
            {
                message = "More than 10 sec for an answer";
            }
            else if (error == "NaN")
            {
                message = "Got NaN from server";
            }
            else if (error == "NIE")
            {
                message = "Invalid Plane position";
            }
            else if (error == "ERR")
            {
                message = "Server returned ERR";
            }
            else if (error == "CNC")
            {
                message = "Error while talking to server";
            }
            else
            {
                message = "Unknown error happened";
            }

            ErrorsQueue.Insert(message + time);
        }


        /*
         * Set components of the client - arrays of lambdas and commands.
         * Also set default error lambda function.
         */
        private void SetClient()
        {
            string[] messages = new string[10];
            Action<double>[] lambdas = new Action<double>[10];

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

        /*
         * A multi-thread safe contatiner for holding errors in a single one list.
         * 
         * author: Jhonny
         * date: 7.4.20
         */
        static class ErrorsQueue
        {
            static private List<string> _list = new List<string>();
            static private object _locker = new object();

            static public List<String> Get()
            {
                lock (_locker)
                {
                    return _list;
                }
            }

            static public void Insert(string message)
            {
                lock (_locker)
                {
                    // Try catch to avoid _list[0] when list empty.
                    try
                    {
                        if (message != _list[0])
                        {
                            _list.Insert(0, message);
                        }
                    }
                    catch (Exception)
                    {
                        _list.Insert(0, message);
                    }
                }
            }
        }
    }
}
