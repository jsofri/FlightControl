using PlaneController.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlaneController.ViewModel
{
    /*
     * Class that serve as a ViewModel layer to transfer data in MVVM.
     * Properties includes getters only, originating from model properties.
     * 
     * author: Jhonny.
     * date: 03.28.20
     */
    public class PlaneViewModel : INotifyPropertyChanged
    {
        private IPlaneModel _model;
        public event PropertyChangedEventHandler PropertyChanged;

        public double VM_HeadingDeg { get { return _model.HeadingDeg; } }
        public double VM_GPSVerticalSpeed { get { return _model.GPSVerticalSpeed; } }
        public double VM_GPSGroundSpeed { get { return _model.GPSGroundSpeed; } }
        public double VM_GPSAltitude { get { return _model.GPSAltitude; } }
        public double VM_PitoSpeed { get { return _model.PitoSpeed; } }
        public double VM_PitoAltitude { get { return _model.PitoAltitude; } }
        public double VM_Roll { get { return _model.Roll; } }
        public double VM_Pitch { get { return _model.Pitch; } }
        public double VM_Latitude { get { return _model.Latitude; } }
        public double VM_Longitude { get { return _model.Longitude; } }
        public String VM_Coords
        {
            get
            {
                String[] coords = new String[] { VM_Latitude.ToString(), VM_Longitude.ToString() };
                return String.Join(", ", coords);
            }
        }
        public List<string> VM_Errors { get { return _model.Errors; } }


        /*
         * Ctor.
         * Get model and set it a delegate to PropertyChangedEventArgs event.
         */
        public PlaneViewModel(IPlaneModel model)
        {
            _model = model;
            _model.PropertyChanged +=
                delegate (object sender, PropertyChangedEventArgs e)
                {
                    // Add "VM_" only to properties.
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        // Notify View on changed properties.
        public void NotifyPropertyChanged(string message)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(message));
            }
        }

        // Might throw Exception.
        public void Connect(string ip, string port)
        {
            _model.Connect(ip, port);
        }

        // Might throw Exception.
        public void Disconnect()
        {
            _model.Disconnect();
        }

        public void SetThrottle(double value)
        {
            _model.SetThrottle(value);
        }

        public void SetAileron(double value)
        {
            _model.SetAileron(value);
        }

        public void SetElevator(double value)
        {
            _model.SetElevator(value);
        }

        public void SetRudder(double value)
        {
            _model.SetRudder(value);
        }
    }
}
