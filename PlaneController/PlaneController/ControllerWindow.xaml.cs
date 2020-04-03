using System.Windows;
using System;
using System.Windows.Input;
using PlaneController.ViewModel;
using PlaneController.Model;
using Microsoft.Maps.MapControl.WPF;
using System.Threading;
using System.ComponentModel;

namespace PlaneController
{
   public partial class ControllerWindow : Window
   {
      PlaneViewModel vm = new PlaneViewModel(new PlaneModel());
      Pushpin AirplanePin = new Pushpin();

      public ControllerWindow(string ip, string port)
      {
         InitializeComponent();
         this.DataContext = this.vm;

         // subscribe 
         vm.PropertyChanged += UpdateCoordsPin;

         // pushpin
         AirplanePin.Location = new Location(vm.VM_Latitude, vm.VM_Longitude);
         AirplanePin.ToolTip = "Airplane";
         AirplanePin.Margin = new Thickness(0, 0, 0, 37);
         this.myMap.Children.Add(AirplanePin);
      }

      private void UpdateCoordsPin(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "VM_Latitude" || e.PropertyName == "VM_Longitude")
         {
            this.myMap.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
               AirplanePin.Location = new Location(vm.VM_Latitude, vm.VM_Longitude);
            }));
         } 
      }

      private void BottomSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         BottomSlider.Value = 0;
      }

      private void LeftSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         LeftSlider.Value = 0;
      }

   }
}
