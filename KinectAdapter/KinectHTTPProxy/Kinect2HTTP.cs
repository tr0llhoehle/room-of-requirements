using KinectFaceTracker;
using System.Drawing;
using System;

namespace KinectHTTPProxy
{
    class Kinect2HTTP
    {
        private Kinect sensor = new Kinect();
        private HTTPConnection connection = new HTTPConnection();
        private MovingAverage avg;
        private ulong lastImageUpdate = 0;
        private ulong updatePeriode = 1000;

        public Kinect2HTTP()
        {
            this.sensor.Setup();

            avg = new MovingAverage(this.sensor);
            this.avg.FaceChanged += this.Face_Changed;
            this.sensor.GestureChanged += this.Gesture_Changed;
            this.sensor.ColorImageChanged += this.ColorImage_Changed;
            this.sensor.DepthImageChanged += this.DepthImage_Changed;
        }

        public System.Windows.Forms.Form DebugWindow
        {
            get { return this.sensor.debugWindow; }
        }

        public void Shutdown()
        {
            sensor.Shutdown();
        }

        private void DepthImage_Changed(object sender, ImageData bm)
        {
            connection.SendDepthImage(bm);
        }

        private void ColorImage_Changed(object sender, ImageData bm)
        {
            if (bm.time - lastImageUpdate > updatePeriode)
            {
                Console.WriteLine(String.Format("img: {0} ms", bm.time - lastImageUpdate));
                connection.SendColorImage(bm);
                lastImageUpdate = bm.time;
            }
        }


        private void Face_Changed(object sender, FaceData faceData)
        {
            connection.SendData(faceData);
        }

        private void Gesture_Changed(object sender, GestureData gestureData)
        {
            connection.SendData(gestureData);
        }
    }
}
