using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlaneController.Resources
{
   /// <summary>
   /// Interaction logic for Joystick.xaml
   /// </summary>
   public partial class Joystick : UserControl//, INotifyPropertyChanged
   {
     
      private Point initPos;
      private double _lastElevator;
      private double _lastRudder;
      //public event PropertyChangedEventHandler PropertyChanged;
      public static readonly DependencyProperty X_Property = DependencyProperty.Register(nameof(X_), typeof(double), typeof(Joystick), new PropertyMetadata((double) 0));
      public static readonly DependencyProperty Y_Property = DependencyProperty.Register(nameof(Y_), typeof(double), typeof(Joystick), new PropertyMetadata((double) 0));


      public Joystick()
      {
         InitializeComponent();
      }

      public double X_
      {
         get
         {
            return (double)GetValue(X_Property);
         }
         set
         {
            SetValue(X_Property, value);
         }
      }

      public double Y_
      {
         get
         {
            return (double)GetValue(Y_Property);
         }
         set
         {
            SetValue(Y_Property, value);
         }
      }

      public void centerKnob_Completed (object sender, EventArgs e)
      {
      }

      private void Knob_Clicked(object sender, MouseButtonEventArgs e)
      {
         // save the coords of the mouse when the knob was clicked
         initPos = e.GetPosition(this);
      }

      private void Knob_Moving(object sender, MouseEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Pressed)
         {
            var x = knobPosition.X;
            var y = knobPosition.Y;
            var maxRadius = Base.Width / 4;
            var radius = Math.Sqrt(x * x + y * y);
            if (radius <= maxRadius)
            {
               knobPosition.X = e.GetPosition(this).X - initPos.X;
               knobPosition.Y = e.GetPosition(this).Y - initPos.Y;

               _lastRudder = knobPosition.X / maxRadius;
               _lastElevator = - knobPosition.Y / maxRadius;

               _lastRudder = (_lastRudder > 1) ? 1 : _lastRudder;
               _lastRudder = (_lastRudder < -1) ? -1 : _lastRudder;
               _lastElevator = (_lastElevator > 1) ? 1 : _lastElevator;
               _lastElevator = (_lastElevator < -1) ? -1 : _lastElevator;
               //PropertyChanged(this, new PropertyChangedEventArgs("Y_"));
            }

         }
      }

      private void Base_MouseUp(object sender, MouseButtonEventArgs e)
      {
         // reset knob
         knobPosition.X = 0;
         knobPosition.Y = 0;

         // update x and y
         X_ = _lastRudder;
         Y_ = _lastElevator;

         // reset x and y
         X_ = 0;
         Y_ = 0;
      }

      private void Base_MouseLeave(object sender, MouseEventArgs e)
      {
         // reset knob
         knobPosition.X = 0;
         knobPosition.Y = 0;

         // update x and y
         X_ = _lastRudder;
         Y_ = _lastElevator;

         // reset x and y
         X_ = 0;
         Y_ = 0;
      }
   }
}
