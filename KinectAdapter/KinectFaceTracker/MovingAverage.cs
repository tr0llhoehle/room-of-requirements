using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectFaceTracker
{
    public class MovingAverage
    {
        private List<FaceData> buffer = new List<FaceData>();
        private int maxBuffer = 10;
        private ulong updatePeriode = 50;
        private ulong lastUpdate = 0;

        public event FaceChangedEventHandler FaceChanged;

        public MovingAverage(Kinect sensor)
        {
            sensor.FaceChanged += Face_Changed;
        }

        private void Face_Changed(object sender, FaceData faceData)
        {
            buffer.Add(faceData);
            if (buffer.Count > maxBuffer)
            {
                buffer.RemoveAt(0);
            }

            FaceData newEstimation = new FaceData();
            newEstimation.happy = faceData.happy;
            newEstimation.wearingGlasses = faceData.wearingGlasses;
            newEstimation.roll = 0;
            newEstimation.pitch = 0;
            newEstimation.yaw = 0;
            newEstimation.time = faceData.time;

            foreach (FaceData data in buffer)
            {
                newEstimation.roll += data.roll;
                newEstimation.pitch += data.pitch;
                newEstimation.yaw += data.yaw;
            }
            newEstimation.roll /= buffer.Count;
            newEstimation.pitch /= buffer.Count;
            newEstimation.yaw /= buffer.Count;

            if (faceData.time - lastUpdate > updatePeriode)
            {
                Console.WriteLine(String.Format("face: {0} ms", faceData.time - lastUpdate));

                FaceChanged(this, newEstimation);
                lastUpdate = faceData.time;
            }
        }
    }
}
