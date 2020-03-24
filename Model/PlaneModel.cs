using System;
using System.ComponentModel;
using System.Threading;

namespace PlaneController.Model
{
    class PlaneModel : IPlaneModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // what to do when there's error from simulator
        public delegate void error();

        private TelnetClient client;
        Thread t;


        private double headingDeg;
        private double gpsVerticalSpeed;
        private double gpsGroundSpeed;
        private double gpsAltitude;
        private double pitoSpeed;
        private double pitoAltitude;
        private double roll;
        private double pitch;

        /*
            throttle   [ 0 , 1 ]

            Aileron   [  -1,  1]

            rudder    [ -1 , 1 ]

            elevator  [-1  , 1]

            "/controls/engines/current-engine/throttle",
	        "/controls/flight/elevator",
            "/controls/flight/rudder",
            "/controls/flight/aileron",


            "/position/latitude-deg",
	        "/position/longitude-deg",
         */
        public double HeadingDeg
        {
            get { return this.headingDeg; }
            set
            {
                if (value != this.headingDeg)
                {
                    this.headingDeg = value;
                    this.Notify("HeadingDeg");
                }
            }
        }

        // Maximum 228 km/h, 123 knots.
        public double GPSVerticalSpeed
        {
            get { return this.gpsVerticalSpeed; }
            set
            {
                if (value != this.gpsVerticalSpeed)
                {
                    this.gpsVerticalSpeed = value;
                    this.Notify("GPSVericalSpeed");
                }
            }
        }

        // Maximum 302 km/h, 163 knots.
        public double GPSGroundSpeed
        {
            get { return this.gpsGroundSpeed; }
            set
            {
                if (value != this.gpsGroundSpeed)
                {
                    this.gpsGroundSpeed = value;
                    this.Notify("GPSGroundSpeed");
                }
            }
        }

        // Maximum 13500 feet
        public double GPSAltitude
        {
            get { return this.gpsAltitude; }
            set
            {
                if (value != this.gpsAltitude)
                {
                    this.gpsAltitude = value;
                    this.Notify("GPSAltitude");
                }
            }
        }

        // Maximum 228 km/h, 123 knots.
        public double PitoSpeed
        {
            get { return this.pitoSpeed; }
            set
            {
                if (value != this.pitoSpeed)
                {
                    this.pitoSpeed = value;
                    this.Notify("PitoSpeed");
                }
            }
        }

        // Maximum 13500 feet
        public double PitoAltitude
        {
            get { return this.pitoAltitude; }
            set
            {
                if (value != this.pitoAltitude)
                {
                    this.pitoAltitude = value;
                    this.Notify("PitoAltitude");
                }
            }
        }


        public double Roll
        {
            get { return this.roll; }
            set
            {
                if (value != this.roll)
                {
                    this.roll = value;
                    this.Notify("Roll");
                }
            }
        }


        public double Pitch
        {
            get { return this.pitch; }
            set
            {
                if (value != this.pitch)
                {
                    this.pitch = value;
                    this.Notify("Pitch");
                }
            }
        }

        public PlaneModel()
        {
            this.client = new TelnetClient();
        }

        public void Connect(string ip, int port)
        {
            this.SetServerRoutine();
            try
            {
                this.client.Connect(ip, port);
                t = new Thread(new ThreadStart(this.client.Run));
                t.Start();
            }
            catch (Exception e)
            {
                if (t.IsAlive)
                    client.Stop();
                t.Abort();
                throw new Exception("error in connecting to server");
            }

        }

        public void Disconnect()
        {
            this.client.Stop();
            t.Join();
        }

        public void SetAileron(double value)
        {
            throw new NotImplementedException();
        }

        public void SetElevator(double value)
        {
            throw new NotImplementedException();
        }

        public void SetRudder(double value)
        {
            throw new NotImplementedException();
        }

        public void SetThrottle(double value)
        {
            throw new NotImplementedException();
        }

        // Can send 9 different strings.
        private void Notify(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void SetServerRoutine()
        {
            string[] messages = new string[8];
            Action<double>[] lambdas = new Action<double>[8];
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

            this.client.SetRoutine(messages, lambdas);
            this.client.SetDefaultErrorAction(() => Notify("Error"));
        }
    }
}
