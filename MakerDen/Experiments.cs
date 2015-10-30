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
            Initialise("Replace with Your Name", CloudMode.None);


            #region paste the code snippet in between the #region and #endregion tags


            while (true)
            {
                Welcome();
            }


            #endregion
        }
    }
}
