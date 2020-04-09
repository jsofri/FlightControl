using PlaneController.Model;
using System;
using System.Collections.Concurrent;
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

        public string VM_HeadingDeg { get { return _model.HeadingDeg.ToString("n2"); } }
        public string VM_GPSVerticalSpeed { get { return _model.GPSVerticalSpeed.ToString("n2"); } }
        public string VM_GPSGroundSpeed { get { return _model.GPSGroundSpeed.ToString("n2"); } }
        public string VM_GPSAltitude { get { return _model.GPSAltitude.ToString("n2"); } }
        public string VM_PitoSpeed { get { return _model.PitoSpeed.ToString("n2"); } }
        public string VM_PitoAltitude { get { return _model.PitoAltitude.ToString("n2"); } }
        public string VM_Roll { get { return _model.Roll.ToString("n2"); } }
        public string VM_Pitch { get { return _model.Pitch.ToString("n2"); } }
        public double VM_Latitude { get { return _model.Latitude; } set { _model.SetLatitude(value); } }
        public double VM_Longitude { get { return _model.Longitude; } set { _model.SetLongitude(value); } }
        public double VM_Rudder
        {
            set
            {
                _model.SetRudder(value);
            }
        }
        public double VM_Elevator
        {
            set
            {
                _model.SetElevator(value);
            }
        }
        public BindingList<string> VM_Errors { get { return new BindingList<string>(_model.Errors); } }

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

        public void SetRudder(double value)
        {
            _model.SetRudder(value);
        }

        // Valid values are [-1, 1].
        public void SetElevator(double value)
        {
            _model.SetElevator(value);
        }

    }
}