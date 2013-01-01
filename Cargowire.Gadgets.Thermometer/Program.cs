using System;
using System.Collections;
using System.Threading;

using global::Gadgeteer.Modules.GHIElectronics;
using global::Gadgeteer.Modules.Seeed;
using global::Gadgeteer.Networking;

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using GT = global::Gadgeteer;
using GTM = global::Gadgeteer.Modules;

// A simple thermometer
namespace Cargowire.Gadgets.Thermometer
{
    public partial class Program
    {
        private Canvas canvas;
        private Window window;
        private Text text;

        /// <summary>This method is run when the mainboard is powered up or reset</summary>
        public void ProgramStarted()
        {
            Debug.Print("Program Started");

            this.canvas = new Canvas();
            this.text = new Text(Resources.GetFont(Resources.FontResources.NinaB), "Starting...");
            this.canvas.Children.Add(this.text);
            
            this.window = this.oledDisplay.WPFWindow;
            this.window.Child = this.canvas;

            this.temperatureHumidity.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(this.TemperatureHumidityMeasurementComplete);
            this.temperatureHumidity.StartContinuousMeasurements();
        }

        /// <summary>When we get a temperature measurement</summary>
        /// <param name="sender">The sender</param>
        /// <param name="temperature">The temperature reading</param>
        /// <param name="relativeHumidity">The relative humidity reading</param>
        private void TemperatureHumidityMeasurementComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            this.text.TextContent = temperature.ToString("F1") + " degrees";            
        }
    }
}
