using KinectFaceTracker;

namespace KinectHTTPProxy
{
    class Kinect2HTTP
    {
        private Kinect sensor = new Kinect();
        private HTTPConnection connection = new HTTPConnection();
        private MovingAverage avg;

        public Kinect2HTTP()
        {
            this.sensor.Setup();

            avg = new MovingAverage(this.sensor);
            this.avg.Changed += this.Sensor_Changed;
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
