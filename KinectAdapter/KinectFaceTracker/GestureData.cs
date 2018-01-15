using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectFaceTracker
{
    public enum GestureType
    {
        Unknown = 0,
        HandsAboveHead = 1,
    };

    public class GestureData
    {
        public GestureType type;
        public ulong id;
        public ulong time;
    };
}
