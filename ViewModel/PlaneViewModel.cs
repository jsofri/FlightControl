using PlaneController.Model;
using System.ComponentModel;

namespace PlaneController.ViewModel
{
    class PlaneViewModel : INotifyPropertyChanged
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

        public PlaneViewModel(IPlaneModel model)
        {
            _model = model;
            _model.PropertyChanged +=
                delegate (object sender, PropertyChangedEventArgs e)
            {
                // Add "VM_" only to properties, not errors.
                foreach (var propInfo in this.GetType().GetProperties())
                {
                    if (propInfo.Name == e.PropertyName)
                    {
                        NotifyPropertyChanged("VM_" + e.PropertyName);
                        return;
                    }
                }

                NotifyPropertyChanged(e.PropertyName);
            };
        }

        public void NotifyPropertyChanged(string message)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(message));
            }
        }
    }
}
