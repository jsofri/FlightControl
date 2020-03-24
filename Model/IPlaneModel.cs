using System.ComponentModel;

namespace PlaneController.Model
{ 
    /*
     * Interface for a plane that serves as a model in MVVM architecture.
     * In order if implementors to work correctly,
     * Plane must be connected to a socket.
     */
    interface IPlaneModel : INotifyPropertyChanged
    {

        // Connect to plane in the given ip and port.
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
