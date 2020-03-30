using System.Windows;
using System.Windows.Input;
using PlaneController.ViewModel;
using PlaneController.Model;

namespace PlaneController
{
   public partial class ControllerWindow : Window
   {
      public ControllerWindow(string ip, string port)
      {
         InitializeComponent();
         PlaneViewModel vm = new PlaneViewModel(new PlaneModel());
         DataContext = vm;
         BottomSlider.Value = 0.5;
      }

      private void BottomSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         BottomSlider.Value = 0;
      }

      private void LeftSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         LeftSlider.Value = 0;
      }

      private void Joystick_Loaded(object sender, RoutedEventArgs e)
      {

      }
   }
}
