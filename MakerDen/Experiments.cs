//file name: Experiments.cs

using Glovebox.Graphics.Grid;
using MakerDen.Sensor;
using System;
using static MakerDen.ConfigurationManager;

namespace MakerDen
{
    public class Experiments : ExplorerHatProServices
    {
        const double LIGHT_THRESHOLD = 60d;

        public void Main()
        {
            Initialise("Faister", CloudMode.IoT_Hub);


            #region paste the code snippet in between the #region and #endregion tags

            using (SensorMgr tempSensor = new SensorMgr(temp))
            {


                tempSensor.OnAfterMeasurement += OnAfterMeasurement;
                tempSensor.OnBeforeMeasurement += OnBeforeMeasure;


                DisplayTemperature().Wait();
            }

            #endregion
        }
    }
}
