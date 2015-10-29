//using Glovebox.HATs;
//using Glovebox.Graphics.Components;
//using Glovebox.Graphics.Drivers;
//using Glovebox.Graphics.Grid;
//using MakerDen.Services;
//using System;
//using System.Diagnostics;

//namespace MakerDen {
//    public class MakerDenServices : ExplorerHatPro {
//        object displayLock = new object();
//        private IServiceManager sm;

//        LED8x8Matrix matrix = new LED8x8Matrix(new Ht16K33());

//        public MakerDenServices(string yourName = "", bool connected = true, bool hat = true) : base(hat) {
//            Util.SetName(yourName);

//            if (connected) {
//                sm = Util.StartNetworkServices(connected);

//                // this is a hack workaround for system time sync issue on W10 for IoT
//                Util.GetTime();
//            }
//        }

//        protected void Welcome() {
//            matrix.ScrollStringInFromRight("Hello " + ConfigurationManager.NetworkId + " I'm " + Util.GetHostName().ToUpper() + " at " + Util.GetIPAddress() + " ", 100);
//        }

//        protected void Display(char c) {
//            matrix.DrawLetter(c); // Light
//            matrix.FrameDraw();
//        }

//        protected void Display(Grid8x8.Symbols sym) {
//            matrix.DrawSymbol(sym);
//            matrix.FrameDraw();
//        }

//        protected void Display(string text) {
//            matrix.FrameClear();
//            matrix.ScrollStringInFromRight(text, 100);
//        }

//        public uint OnBeforeMeasure(object sender, EventArgs e) {
//            uint id = ((SensorMgr.SensorIdEventArgs)e).id;
//            if (Led == null) { return 0; }
//            Led[id % 2].On();
//            Util.Delay(5);
//            Led[id % 2].Off();
//            return 0;
//        }

//        public uint SetLEDMatrixBrightness(object sender, EventArgs e) {
//            if (matrix == null) { return 0; }
//            var data = ((SensorMgr.SensorItemEventArgs)e).data;
//            matrix.SetBrightness((byte)((data.Values()[0] % 100) / 35));
//            return 0;
//        }

//        protected uint OnAfterMeasurement(object sender, EventArgs e) {
//            uint result;
//            var data = ((SensorMgr.SensorItemEventArgs)e).data;
//            var json = data.ToJson();

//            matrix.DrawLetter(data.MeasureName.ToUpper()[0]);
//            matrix.FrameDraw();

//            if (sm == null) {
//                Debug.WriteLine(data.ToString());
//                return 0;
//            }

//            var topic = data.Topic;

//            result = sm.Publish(topic, json);

//            Util.Delay(100);

//            return result;
//        }
//    }
//}
