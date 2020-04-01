using PlaneController.ViewModel;
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
        private PlaneViewModel _vm;

        private double _throttle;

        private double _aileron;

        // Ctor - gets a ViewModel object as a parameter.
        public ControllerWindow(PlaneViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = _vm;
            _throttle = ThrottleSlider.Value = 0.5;
            _aileron = AileronSlider.Value = 0;
            _vm.PropertyChanged +=
                   delegate (object sender, PropertyChangedEventArgs e)
                   {
                       // Error occured
                       if (e.PropertyName == "VM_Errors")
                       {
                           this.Dispatcher.Invoke(() =>
                           {
                               ErrorsComboBox.ItemsSource = vm.VM_Errors;
                               if (vm.VM_Errors.Count > 0)
                               {
                                   ErrorsLabel.Foreground = Brushes.Red;
                                   ErrorsComboBox.SelectedIndex = 0;
                               }
                           });

                       }
                   };
        }

        private void Joystick_Loaded(object sender, RoutedEventArgs e)
        {

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
            if (diff >= 0.05 || diff <= -0.05)
            {
                _throttle = ThrottleSlider.Value;
                _vm.SetThrottle(_throttle);
            }
        }
    }
}
