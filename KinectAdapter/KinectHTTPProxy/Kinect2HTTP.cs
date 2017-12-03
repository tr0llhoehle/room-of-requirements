using KinectFaceTracker;

namespace KinectHTTPProxy
{
    class Kinect2HTTP
    {
        private Kinect sensor = new Kinect();
        private HTTPConnection connection = new HTTPConnection();

        public Kinect2HTTP()
        {
            this.sensor.Setup();

            this.sensor.Changed += this.Sensor_Changed;
        }

        public void Shutdown()
        {
            sensor.Shutdown();
        }

        private void Sensor_Changed(object sender, FaceData faceData)
        {
            connection.SendData(faceData);
        }
    }
}
