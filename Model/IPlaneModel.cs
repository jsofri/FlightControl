using System.ComponentModel;

namespace PlaneController.Model
{
    //add documentation and explnation about methods (e.g. left + right -)
    interface IPlaneModel : INotifyPropertyChanged
    {
        
        //for plane server
        void Connect(string ip, int port);
        void Disconnect();

        //mitzeret
        void SetThrottle(double value);
        //meaznot
        void SetAileron(double value);
        //hege gova
        void SetElevator(double value);
        //hege kivun
        void SetRudder(double value);
    }
}
