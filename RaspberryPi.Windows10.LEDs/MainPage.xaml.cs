using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspberryPi.Windows10.LEDs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int REDLEDPIN = 26;
        private const int GREENLEDPIN = 12;
        private const int BLUELEDPIN = 16;
        private const int YELLOWLEDPIN = 25;
        private const int RGBREDLEDPIN = 5;
        private const int RGBGREENLEDPIN = 6;
        private const int RGBBLUELEDPIN = 13;

        private GpioPin RedLED = null;
        private GpioPin GreenLED = null;
        private GpioPin BlueLED = null;
        private GpioPin YellowLED = null;
        private GpioPin RGBRedLED = null;
        private GpioPin RGBGreenLED = null;
        private GpioPin RGBBlueLED = null;

        private GpioController gpio = null;

        // LED on -> GpioPinValue.Low
        private const GpioPinValue LEDHIGH = GpioPinValue.Low;
        private const GpioPinValue LEDLOW = GpioPinValue.High;

        private SolidColorBrush RedBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush GreenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush BlueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush YellowBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private SolidColorBrush GrayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            this.InitializeComponent();

            Unloaded += MainPage_Unloaded;

            //Initialize GPIO
            InitGPIO();
            if (gpio == null)
            {
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            // Initialize pins
            InitPin(ref RedLED, REDLEDPIN);
            InitPin(ref GreenLED, GREENLEDPIN);
            InitPin(ref BlueLED, BLUELEDPIN);
            InitPin(ref YellowLED, YELLOWLEDPIN);
            InitPin(ref RGBRedLED, RGBREDLEDPIN);
            InitPin(ref RGBGreenLED, RGBGREENLEDPIN);
            InitPin(ref RGBBlueLED, RGBBLUELEDPIN);

            if (RedLED == null || GreenLED == null || BlueLED == null || YellowLED == null || RGBRedLED == null || RGBGreenLED == null || RGBBlueLED == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO pins.";
                return;
            }

            GpioStatus.Text = "GPIO pins initialized correctly.";
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            RedLED.Dispose();
            GreenLED.Dispose();
            BlueLED.Dispose();
            YellowLED.Dispose();
            RGBRedLED.Dispose();
            RGBGreenLED.Dispose();
            RGBBlueLED.Dispose();
        }
        public void InitGPIO()
        {
            //Get GPIO Controller
            gpio = GpioController.GetDefault();
        }
        public void InitPin(ref GpioPin pin, int pinNumber)
        {
            // Open Pin
            pin = gpio.OpenPin(pinNumber);

            if (pin == null)
                return;

            // Set Pin to Output
            pin.SetDriveMode(GpioPinDriveMode.Output);

            // Turn off LED
            TurnLedOff(ref pin, "");
        }
        public void TurnLedOn(ref GpioPin pin, string ledName)
        {
            pin.Write(LEDHIGH);
            ChangeEllipseColor(ledName, true);
        }
        public void TurnLedOff(ref GpioPin pin, string ledName)
        {
            pin.Write(LEDLOW);
            ChangeEllipseColor(ledName, false);
        }
        private void FlipLED(ref GpioPin pin, string ledName)
        {
            var v = pin.Read();
            if (v == LEDHIGH)
                TurnLedOff(ref pin, ledName);
            else
                TurnLedOn(ref pin, ledName);
        }
        public void TurnRGBLedOn(ref GpioPin pin, string ledName)
        {
            pin.Write(GpioPinValue.High);
            ChangeEllipseColor(ledName, true);
        }
        public void TurnRGBLedOff(ref GpioPin pin, string ledName)
        {
            pin.Write(GpioPinValue.Low);
            ChangeEllipseColor(ledName, false);
        }
        private void FlipRGBLED(ref GpioPin pin, string ledName)
        {
            var v = pin.Read();
            if (v == GpioPinValue.High)
                TurnRGBLedOff(ref pin, ledName);
            else
                TurnRGBLedOn(ref pin, ledName);
        }

        private void ChangeEllipseColor(string ledName, bool High)
        {
            if (ledName == "")
                return;

            Ellipse el = this.FindName(ledName) as Ellipse;

            if (!High)
            {
                el.Fill = GrayBrush;
                return;
            }

            //if (ledName.Contains("Red"))
            //    el.Fill = RedBrush;
            //else if (ledName.Contains("Green"))
            //    el.Fill = GreenBrush;
            //else if (ledName.Contains("Blue"))
            //    el.Fill = BlueBrush;
            //else if (ledName.Contains("Yellow"))
            //    el.Fill = YellowBrush;

            if (ledName == "eRedLED")
                el.Fill = RedBrush;
            else if (ledName == "eGreenLED")
                el.Fill = GreenBrush;
            else if (ledName == "eBlueLED")
                el.Fill = BlueBrush;
            else if (ledName == "eYellowLED")
                el.Fill = YellowBrush;
            else if (ledName == "eRGBRedLED")
                el.Fill = RedBrush;
            else if (ledName == "eRGBGreenLED")
                el.Fill = GreenBrush;
            else if (ledName == "eRGBBlueLED")
                el.Fill = BlueBrush;
        }

        private void EllipseTapped(object sender, TappedRoutedEventArgs e)
        {
            string t = (sender as Ellipse).Name;

            if (t == "eRedLED")
                FlipLED(ref RedLED, t);
            else if (t == "eGreenLED")
                FlipLED(ref GreenLED, t);
            else if (t == "eBlueLED")
                FlipLED(ref BlueLED, t);
            else if (t == "eYellowLED")
                FlipLED(ref YellowLED, t);
            else if (t == "eRGBRedLED")
                FlipRGBLED(ref RGBRedLED, t);
            else if (t == "eRGBGreenLED")
                FlipRGBLED(ref RGBGreenLED, t);
            else if (t == "eRGBBlueLED")
                FlipRGBLED(ref RGBBlueLED, t);
        }
    }
}
