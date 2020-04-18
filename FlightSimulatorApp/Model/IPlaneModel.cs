using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace FlightSimulator.Model
{
    /*
     * Interface for a plane that serves as a model in MVVM architecture.
     * In order if implementors to work correctly,
     * Plane must be connected to a socket.
     * 
     * author: Jhonny.
     * date: 3.28.20
     */
    public interface IPlaneModel : INotifyPropertyChanged
    {

        double HeadingDeg { get; set; }
        double GPSVerticalSpeed { get; set; }
        double GPSGroundSpeed { get; set; }
        double GPSAltitude { get; set; }
        double PitoSpeed { get; set; }
        double PitoAltitude { get; set; }
        double Roll { get; set; }
        double Pitch { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        List<string> Errors { get; }

        /* 
         * Connect to plane in the given ip and port.
         * Throws exception if there's a problem.
         */
        void Connect(string ip, string port);

        // Disconnect from plane.
        void Disconnect();

        // Valid values are [0, 1].
        void SetThrottle(double value);

        // Valid values are [-1, 1].
        void SetAileron(double value);

        // Valid values are [-1, 1].
        void SetElevator(double value);

        // Valid values are [-1, 1].
        void SetRudder(double value);
   }
}
