using Microsoft.Maps.MapControl.WPF;
using PlaneController.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace PlaneController
{

    /*
     * Class is a control window in a plane simulator.
     * Function as a View class in MVVM architecture.
     * 
     * author: Jhonny.
     * date: 4.1.20
     */
    public partial class ControllerWindow : Window
    {
        private volatile PlaneViewModel _vm;

        private double _throttle;

        private double _aileron;

        private Pushpin _pin = new Pushpin();

        private const double EPSILON = 0.00001;



       // Ctor - gets a ViewModel object as a parameter.
       public ControllerWindow(PlaneViewModel vm)
       {
            InitializeComponent();
            _vm = vm;
            this.DataContext = this._vm;

            // set default values to sliders
            _throttle = ThrottleSlider.Value = 0.5;
            _aileron = AileronSlider.Value = 0;

            // subscribe to coords change
            _vm.PropertyChanged += UpdateCoordsPin;
            this.SetErrorEvent();

            // add pushpin to map
            _pin.Location = new Location(vm.VM_Latitude, vm.VM_Longitude);
            _pin.ToolTip = "Airplane";
            this.myMap.Children.Add(_pin);
        }

        /*
         * Update the pushpin's location on map and center the map view on the coords
         */
        private void UpdateCoordsPin(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VM_Latitude" || e.PropertyName == "VM_Longitude")
            {
                this.myMap.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                {
                    // update pushpin's location
                    _pin.Location = new Location(_vm.VM_Latitude, _vm.VM_Longitude);

                    // center map view
                    myMap.Center = new Location(_vm.VM_Latitude, _vm.VM_Longitude);
                }));
            }
        }

        // Return "Errors" label to regular color.
        private void ErrorsBox_Opened(object sender, System.EventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#FFE5D5D5");

            ErrorsLabel.Foreground = new SolidColorBrush(color);
        }

        /*
         * Event handler for aileron slider object. gets called a lot.
         * As a result, call to VM only if the difference from value is at least 0.05
         */
        private void AileronSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double diff = AileronSlider.Value - _aileron;

            // If diff is at least 0.05 (absolute value).
            if (diff >= 0.05 || diff <= -0.05)
            {
                _aileron = AileronSlider.Value;
                _vm.SetAileron(_aileron);
            }
        }

        /*
         * Event handler for throttle slider object. gets called a lot.
         * As a result, call to VM only if the difference from value is at least 0.05
         */
        private void ThrottleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double diff = ThrottleSlider.Value - _throttle;

            // If diff is at least 0.05 (absolute value).
            if (Math.Abs(diff) >= 0.05)
            {
                _throttle = ThrottleSlider.Value;
                _vm.SetThrottle(_throttle);
            }
        }

        private void SetErrorEvent()
        {
            _vm.PropertyChanged +=
                delegate (object sender, PropertyChangedEventArgs e)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (_vm.VM_Errors.Count > 0)
                        {
                            ErrorsLabel.Foreground = Brushes.Red;
                        }
                    });
                };
        }
    }
}

