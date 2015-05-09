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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspberryPi.Windows10.LEDs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int LEDPIN = 5;
        private GpioPin led = null;
        private GpioController gpio = null;

        public MainPage()
        {
            this.InitializeComponent();

            Unloaded += MainPage_Unloaded;

            InitGPIO();

            TurnLedOn(ref led, LEDPIN);
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            led.Dispose();
        }

        public void InitGPIO()
        {
            gpio = GpioController.GetDefault();

            if(gpio == null)
            {
                return;
            }
        }

        public void TurnLedOn(ref GpioPin pin, int pinNumber)
        {
            pin = gpio.OpenPin(pinNumber);

            if (pin == null)
            {
                return;
            }

            pin.SetDriveMode(GpioPinDriveMode.Output);
            // Pin needs to be low in order to go on
            pin.Write(GpioPinValue.Low);
        }
    }
}
