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
            //return knobPosition.X;
            return (double)GetValue(X_Property);
         }
         set
         {
            //knobPosition.X = value;
            SetValue(X_Property, value);
         }
      }

      public double Y_
      {
         get
         {
            //return knobPosition.Y;
            return (double)GetValue(Y_Property);
         }
         set
         {
            //knobPosition.Y = value;
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
               Console.WriteLine(knobPosition.X + ",  " + knobPosition.Y);
               X_ = knobPosition.X / maxRadius;
               Y_ = - knobPosition.Y / maxRadius;

               X_ = (X_ > 1) ? 1 : X_;
               X_ = (X_ < -1) ? -1 : X_;
               Y_ = (Y_ > 1) ? 1 : Y_;
               Y_ = (Y_ < -1) ? -1 : Y_;
               //PropertyChanged(this, new PropertyChangedEventArgs("Y_"));
            }

         }
      }

      private void Base_MouseUp(object sender, MouseButtonEventArgs e)
      {
         knobPosition.X = 0;
         knobPosition.Y = 0;
         X_ = knobPosition.X;
         Y_ = knobPosition.Y;
      }

      private void Base_MouseLeave(object sender, MouseEventArgs e)
      {
         knobPosition.X = 0;
         knobPosition.Y = 0;
         X_ = knobPosition.X;
         Y_ = knobPosition.Y;
      }
   }
}
