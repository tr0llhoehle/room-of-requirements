using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace KinectFaceTracker
{
    class GestureRecognition
    {
        public GestureType gesture = GestureType.Unknown;

        public bool Update(Body body)
        {
            bool changed = false;
            if (body.IsTracked)
            {
                Joint leftHand = body.Joints[JointType.HandLeft];
                Joint rightHand = body.Joints[JointType.HandRight];
                Joint head = body.Joints[JointType.Head];
                bool handsAboveHead = leftHand.Position.Y > head.Position.Y && rightHand.Position.Y > head.Position.Y;
                bool handsBesidesHead = leftHand.Position.X < head.Position.X && rightHand.Position.X > head.Position.X;

                if (handsAboveHead && handsBesidesHead)
                {
                    changed = gesture != GestureType.HandsAboveHead;
                    gesture = GestureType.HandsAboveHead;
                }
                else
                {
                    changed = gesture != GestureType.Unknown;
                    gesture = GestureType.Unknown;
                }
            }

            return changed;
        }
    }
}
