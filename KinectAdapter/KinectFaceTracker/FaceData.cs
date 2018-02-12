using Microsoft.Kinect;

namespace KinectFaceTracker
{
    public class FaceData
    {
        public DetectionResult happy = DetectionResult.Unknown;
        public DetectionResult wearingGlasses = DetectionResult.Unknown;
        public int roll;
        public int pitch;
        public int yaw;
        public double height;
        public ulong id;
        public ulong time;
    };
}
