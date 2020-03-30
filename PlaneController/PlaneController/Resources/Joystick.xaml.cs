using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlaneController.Resources
{
   /// <summary>
   /// Interaction logic for Joystick.xaml
   /// </summary>
   public partial class Joystick : UserControl
   {
      Point initPos;

      public Joystick()
      {
         InitializeComponent();
      }

      public void centerKnob_Completed (object sender, EventArgs e)
      {
      }

      private void Knob_Clicked(object sender, MouseButtonEventArgs e)
      {
         // save the coords of the mouse when the knob was clicked
         initPos = e.GetPosition(Knob);
      }

      private void Knob_Moving(object sender, MouseEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Pressed)
         {
            this.knobDragging();
         }
      }

      private void knobDragging()
      {
         
      }
   }
}
