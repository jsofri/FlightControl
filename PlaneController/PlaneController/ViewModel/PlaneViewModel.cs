using PlaneController.Model;
using System;
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
   class PlaneViewModel : INotifyPropertyChanged
   {
      private PlaneModel _model;
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
      public String VM_Coords {
         get {
            String[] coords = new String[] { VM_Latitude.ToString(), VM_Longitude.ToString() };
            return String.Join(", ", coords);
         }
      }
      public double VM_Rudder {
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

      /*
       * Ctor.
       * Get model and set it a delegate to PropertyChangedEventArgs event.
       */
      public PlaneViewModel(PlaneModel model)
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

      // Notify View on changed properties.
      public void NotifyPropertyChanged(string message)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(message));
         }
      }

      // Might throw Exception.
      void Connect(string ip, string port)
      {
         _model.Connect(ip, port);
      }

      // Might throw Exception.
      void Disconnect()
      {
         _model.Disconnect();
      }

      void SetThrottle(double value)
      {
         _model.SetThrottle(value);
      }

      void SetAileron(double value)
      {
         _model.SetAileron(value);
      }

   }
}