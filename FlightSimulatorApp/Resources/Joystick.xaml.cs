using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FlightSimulator.Resources
{
    /*
     * Class that serves as a the code-behind of the Joystick.
     * 
     * author: Rony.
     * date: 03.28.20
     */
    public partial class Joystick : UserControl
    {
        private double _angle;
        private double _radius;
        private const double NO_ANGLE = 999;
        private const double NO_RADIUS = 0;
        private Point _InitPos;
        private const double EPSILON = 0.00001;
        public static readonly DependencyProperty X_Property = DependencyProperty.Register(nameof(X_), typeof(double), typeof(Joystick), new PropertyMetadata((double)0));
        public static readonly DependencyProperty Y_Property = DependencyProperty.Register(nameof(Y_), typeof(double), typeof(Joystick), new PropertyMetadata((double)0));

        // Ctor
        public Joystick()
        {
            InitializeComponent();
            _angle = NO_ANGLE;
            _radius = NO_RADIUS;
        }

        // Rudder property
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

        // Elevator property
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

        // the on-complete event - should stay empty
        public void centerKnob_Completed(object sender, EventArgs e) {
            Reset();
        }

        // on click event
        private void Knob_Clicked(object sender, MouseButtonEventArgs e)
        {
            // save the coords of the mouse when the knob was clicked
            _InitPos = e.GetPosition(this);
            Knob.CaptureMouse();
        }

        // on mouse move event
        private void Knob_Moving(object sender, MouseEventArgs e)
        {
            // check if left button is being pressed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                KnobInRange(e);

                var maxRadius = Base.Width / 4;

                // normalize the x and y to be in the range [-1, 1]
                X_ = knobPosition.X / maxRadius;
                Y_ = -knobPosition.Y / maxRadius;

                // keep them in range [-1, 1]
                X_ = (X_ > 1) ? 1 : X_;
                X_ = (X_ < -1) ? -1 : X_;
                Y_ = (Y_ > 1) ? 1 : Y_;
                Y_ = (Y_ < -1) ? -1 : Y_;
            }
        }

        // when the mouse button is released
        private void Base_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // do the animation
            Storyboard animation = (Storyboard) Knob.FindResource("CenterKnob");
            animation.FillBehavior = FillBehavior.Stop;
            animation.Begin();

            // release knob capture
            Knob.ReleaseMouseCapture();

            // reset angle
            _angle = NO_ANGLE;
            _radius = NO_RADIUS;
        }

        // when the mouse leaves
        private void Base_MouseLeave(object sender, MouseEventArgs e)
        {
            // check if left button is being pressed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                KnobInRange(e);
            }
        }

        // keep the knob inside the base
        private void KnobInRange(MouseEventArgs e)
        {

            var maxRadius = Base.Width / 4;
            knobPosition.X = e.GetPosition(this).X - _InitPos.X;
            knobPosition.Y = e.GetPosition(this).Y - _InitPos.Y;

            var x = knobPosition.X;
            var y = -knobPosition.Y;
            var radius = Math.Sqrt(x * x + y * y);
            double angle, maxX, maxY;

            // if initial angle is yet to be set
            if (_angle == NO_ANGLE)
            {
                // avoid dividing by 0
                if (x != 0)
                {
                    angle = Math.Atan(y / x);
                    maxX = maxRadius * Math.Cos(angle);
                    maxY = maxRadius * Math.Sin(angle);
                }
                else
                {
                    if (y == 0)
                    {
                        return;
                    }

                    angle = 0;
                    maxX = 0;
                    maxY = maxRadius;
                }

                // only if the radius is big enough - record the angle
                if (radius > KnobBase.Width / 4)
                {
                    angle = (angle < 0) ? (Math.PI / 2) + angle : angle;
                    // convert angle according to quarter
                    _angle = Math.Abs(angle) + (Math.PI / 2) * (getQuarter(x, y) - 1);
                    _radius = radius;
                }
            }
            else
            {
                maxX = maxRadius * Math.Cos(_angle);
                maxY = maxRadius * Math.Sin(_angle);

                // update radius
                _radius = (radius > _radius && radius <= maxRadius) ? radius : _radius;

                // place knob in the initial angle
                x = _radius * Math.Cos(_angle);
                y = _radius * Math.Sin(_angle);
                knobPosition.X = x;
                knobPosition.Y = -y;
                
            }

            // if knob exceeds boundaries
            if (radius > maxRadius)
            {
                knobPosition.X = maxX;
                knobPosition.Y = -maxY;
            }
        }

        private void Reset() {
            // reset knob
            knobPosition.X = 0;
            knobPosition.Y = 0;
            
            // reset properties
            X_ = 0;
            Y_ = 0;
        }

        private int getQuarter(double x, double y)
        {
            // 1st quarter
            if (x > 0 && y >= 0)
            {
                return 1;
            }
            // 2nd quarter
            else if (x <= 0 && y > 0)
            {
                return 2;
            }
            // 3rd quarter
            else if (x < 0 && y <= 0)
            {
                return 3;
            }
            // 4th quarter
            else if (x >= 0 && y < 0)
            {
                return 4;
            }
            // no quarter
            else
            {
                return 0;
            }
        }
    }
}