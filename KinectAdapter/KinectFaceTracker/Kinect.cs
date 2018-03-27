//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace KinectFaceTracker
{
    using System;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Face;
    using System.Windows;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    public delegate void FaceChangedEventHandler(object sender, FaceData e);
    public delegate void GestureChangedEventHandler(object sender, GestureData e);
    public delegate void ColorImageEventHandler(object sender, ImageData e);
    public delegate void DepthFrameEventHandler(object sender, ImageData e);

    public class Kinect
    {
        public event FaceChangedEventHandler FaceChanged;
        public event GestureChangedEventHandler GestureChanged;
        public event ColorImageEventHandler ColorImageChanged;
        public event DepthFrameEventHandler DepthImageChanged;

        /// <summary>
        /// Face rotation display angle increment in degrees
        /// </summary>
        private const double FaceRotationIncrementInDegrees = 5.0;
        
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        private ColorFrameReader colorFrameReader = null;
        private DepthFrameReader depthFrameReader = null;

        /// <summary>
        /// Array to store bodies
        /// </summary>
        private Body[] bodies = null;
        private GestureRecognition[] gestureTrackers = null;

        /// <summary>
        /// Number of bodies tracked
        /// </summary>
        private int bodyCount;

        /// <summary>
        /// Face frame sources
        /// </summary>
        private FaceFrameSource[] faceFrameSources = null;

        /// <summary>
        /// Face frame readers
        /// </summary>
        private FaceFrameReader[] faceFrameReaders = null;

        /// <summary>
        /// Storage for face frame results
        /// </summary>
        private FaceFrameResult[] faceFrameResults = null;

        /// <summary>
        /// Width of display (color space)
        /// </summary>
        private int displayWidth;

        /// <summary>
        /// Height of display (color space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// Display rectangle
        /// </summary>
        private Rect displayRect;

        public DebugWindow debugWindow;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public Kinect()
        {
            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get the color frame details
            FrameDescription frameDescription = this.kinectSensor.ColorFrameSource.FrameDescription;

            // set the display specifics
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;
            this.displayRect = new Rect(0.0, 0.0, this.displayWidth, this.displayHeight);

            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();
            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            // FIXME Does not work
            //this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();
            //this.depthFrameReader.FrameArrived += this.Reader_DepthFrameArrived;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // wire handler for body frame arrival
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

            // set the maximum number of bodies that would be tracked by Kinect
            this.bodyCount = this.kinectSensor.BodyFrameSource.BodyCount;

            // allocate storage to store body objects
            this.bodies = new Body[this.bodyCount];
            this.gestureTrackers = new GestureRecognition[this.bodyCount];

            this.debugWindow = new DebugWindow();
            this.debugWindow.Size = new System.Drawing.Size(800, 600);
            this.debugWindow.Show();

            // specify the required face frame results
            FaceFrameFeatures faceFrameFeatures =
                FaceFrameFeatures.BoundingBoxInColorSpace
                | FaceFrameFeatures.PointsInColorSpace
                | FaceFrameFeatures.RotationOrientation
                | FaceFrameFeatures.FaceEngagement
                | FaceFrameFeatures.Glasses
                | FaceFrameFeatures.Happy
                | FaceFrameFeatures.LeftEyeClosed
                | FaceFrameFeatures.RightEyeClosed
                | FaceFrameFeatures.LookingAway
                | FaceFrameFeatures.MouthMoved
                | FaceFrameFeatures.MouthOpen;

            // create a face frame source + reader to track each face in the FOV
            this.faceFrameSources = new FaceFrameSource[this.bodyCount];
            this.faceFrameReaders = new FaceFrameReader[this.bodyCount];
            for (int i = 0; i < this.bodyCount; i++)
            {
                // create the face frame source with the required face frame features and an initial tracking Id of 0
                this.faceFrameSources[i] = new FaceFrameSource(this.kinectSensor, 0, faceFrameFeatures);

                // open the corresponding reader
                this.faceFrameReaders[i] = this.faceFrameSources[i].OpenReader();

                this.gestureTrackers[i] = new GestureRecognition();
            }

            // allocate storage to store face frame results for each face in the FOV
            this.faceFrameResults = new FaceFrameResult[this.bodyCount];

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();
        }

        /// <summary>
        /// Converts rotation quaternion to Euler angles 
        /// And then maps them to a specified range of values to control the refresh rate
        /// </summary>
        /// <param name="rotQuaternion">face rotation quaternion</param>
        /// <param name="pitch">rotation about the X-axis</param>
        /// <param name="yaw">rotation about the Y-axis</param>
        /// <param name="roll">rotation about the Z-axis</param>
        private static void ExtractFaceRotationInDegrees(Vector4 rotQuaternion, out int pitch, out int yaw, out int roll)
        {
            double x = rotQuaternion.X;
            double y = rotQuaternion.Y;
            double z = rotQuaternion.Z;
            double w = rotQuaternion.W;

            // convert face rotation quaternion to Euler angles in degrees
            double yawD, pitchD, rollD;
            pitchD = Math.Atan2(2 * ((y * z) + (w * x)), (w * w) - (x * x) - (y * y) + (z * z)) / Math.PI * 180.0;
            yawD = Math.Asin(2 * ((w * y) - (x * z))) / Math.PI * 180.0;
            rollD = Math.Atan2(2 * ((x * y) + (w * z)), (w * w) + (x * x) - (y * y) - (z * z)) / Math.PI * 180.0;

            // clamp the values to a multiple of the specified increment to control the refresh rate
            double increment = FaceRotationIncrementInDegrees;
            pitch = (int)(Math.Floor((pitchD + ((increment / 2.0) * (pitchD > 0 ? 1.0 : -1.0))) / increment) * increment);
            yaw = (int)(Math.Floor((yawD + ((increment / 2.0) * (yawD > 0 ? 1.0 : -1.0))) / increment) * increment);
            roll = (int)(Math.Floor((rollD + ((increment / 2.0) * (rollD > 0 ? 1.0 : -1.0))) / increment) * increment);
        }

        protected virtual void OnColorImageChanged(ImageData e)
        {
            ColorImageChanged.Invoke(this, e);
        }

        protected virtual void OnDepthImageChanged(ImageData e)
        {
            DepthImageChanged.Invoke(this, e);
        }

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnFaceChanged(FaceData e)
        {

            FaceChanged.Invoke(this, e);
        }

        protected virtual void OnGestureChanged(GestureData e)
        {

            GestureChanged.Invoke(this, e);
        }

        public void Setup()
        {
            for (int i = 0; i < this.bodyCount; i++)
            {
                if (this.faceFrameReaders[i] != null)
                {
                    // wire handler for face frame arrival
                    this.faceFrameReaders[i].FrameArrived += this.Reader_FaceFrameArrived;
                }
            }

            if (this.bodyFrameReader != null)
            {
                // wire handler for body frame arrival
                this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;
            }
        }


        public void Shutdown()
        {
            for (int i = 0; i < this.bodyCount; i++)
            {
                if (this.faceFrameReaders[i] != null)
                {
                    // FaceFrameReader is IDisposable
                    this.faceFrameReaders[i].Dispose();
                    this.faceFrameReaders[i] = null;
                }

                if (this.faceFrameSources[i] != null)
                {
                    // FaceFrameSource is IDisposable
                    this.faceFrameSources[i].Dispose();
                    this.faceFrameSources[i] = null;
                }
            }

            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            Bitmap target = new Bitmap(section.Width, section.Height, source.PixelFormat);

            Graphics g = Graphics.FromImage(target);
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return target;
        }

        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            for (int i = 0; i < this.bodyCount; i++)
            {
                if (this.faceFrameSources[i].IsTrackingIdValid && this.faceFrameResults[i] != null)
                {
                    ColorFrame frame = e.FrameReference.AcquireFrame();

                    using (frame)
                    {
                        // Next get the frame's description and create an output bitmap image.
                        FrameDescription description = frame.FrameDescription;
                        var outputImage = new Bitmap(description.Width, description.Height, PixelFormat.Format32bppArgb);

                        // Next, we create the raw data pointer for the bitmap, as well as the size of the image's data.
                        var imageData = outputImage.LockBits(new Rectangle(0, 0, outputImage.Width, outputImage.Height),
                            ImageLockMode.WriteOnly, outputImage.PixelFormat);
                        IntPtr imageDataPtr = imageData.Scan0;
                        int size = imageData.Stride * outputImage.Height;

                        // After this, we copy the image data directly to the buffer.  Note that while this is in BGRA format, it will be flipped due
                        // to the endianness of the data.
                        if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
                        {
                            frame.CopyRawFrameDataToIntPtr(imageDataPtr, (uint)size);
                        }
                        else
                        {
                            frame.CopyConvertedFrameDataToIntPtr(imageDataPtr, (uint)size, ColorImageFormat.Bgra);
                        }

                        // Finally, unlock the output image's raw data again and create a new bitmap for the preview picture box.
                        outputImage.UnlockBits(imageData);

                        var rect = this.faceFrameResults[i].FaceBoundingBoxInColorSpace;
                        Rectangle bbox = new Rectangle(Math.Max(0, rect.Left - 30), Math.Max(0, rect.Top-30),
                                                       Math.Min(rect.Right - rect.Left + 60, outputImage.Width),
                                                       Math.Min(rect.Bottom - rect.Top + 60, outputImage.Height));
                        Bitmap cropped = CropImage(outputImage, bbox);

                        if (this.debugWindow != null)
                        {
                            this.debugWindow.UpdateFrame(cropped);
                        }

                        ImageData data = new ImageData();
                        data.image = cropped;
                        data.id = this.faceFrameResults[i].TrackingId;
                        data.time = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

                        OnColorImageChanged(data);
                
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the face frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame != null)
                {
                    // get the index of the face source from the face source array
                    int index = this.GetFaceSourceIndex(faceFrame.FaceFrameSource);

                    // check if this face frame has valid face frame results
                    if (this.ValidateFaceBoxAndPoints(faceFrame.FaceFrameResult))
                    {
                        // store this face frame result to draw later
                        this.faceFrameResults[index] = faceFrame.FaceFrameResult;
                    }
                    else
                    {
                        // indicates that the latest face frame result from this reader is invalid
                        this.faceFrameResults[index] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the index of the face frame source
        /// </summary>
        /// <param name="faceFrameSource">the face frame source</param>
        /// <returns>the index of the face source in the face source array</returns>
        private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
        {
            int index = -1;

            for (int i = 0; i < this.bodyCount; i++)
            {
                if (this.faceFrameSources[i] == faceFrameSource)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        double EuclideanDistance(CameraSpacePoint p1, CameraSpacePoint p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Z - p2.Z, 2) + Math.Pow(p1.Z - p2.Z, 2));
        }

        delegate double Distance(JointType a, JointType b);
        private double MeasureHeight(Body body)
        {
            Distance d = (a, b) =>
            {
                var p1 = body.Joints[a].Position;
                var p2 = body.Joints[b].Position;
                return EuclideanDistance(p1, p2);
            };
            double torso_height = d(JointType.Head, JointType.Neck) +
               d(JointType.Neck, JointType.SpineShoulder) +
               d(JointType.SpineShoulder, JointType.SpineMid) +
               d(JointType.SpineMid, JointType.SpineBase) +
               0.5 * d(JointType.SpineMid, JointType.HipRight) + 0.5 * d(JointType.SpineMid, JointType.HipRight);

            double left_leg_height = d(JointType.HipLeft, JointType.KneeLeft) +
                              d(JointType.KneeLeft, JointType.AnkleLeft) +
                              d(JointType.AnkleLeft, JointType.FootLeft);

            double right_leg_height = d(JointType.HipRight, JointType.KneeRight) +
                           d(JointType.KneeRight, JointType.AnkleRight) +
                           d(JointType.AnkleRight, JointType.FootRight);

            return torso_height + (left_leg_height + right_leg_height) / 2.0;
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    // update body data
                    bodyFrame.GetAndRefreshBodyData(this.bodies);

                    // iterate through each face source
                    for (int i = 0; i < this.bodyCount; i++)
                    {
                        bool changed = this.gestureTrackers[i].Update(this.bodies[i]);
                        if (changed)
                        {
                            GestureData g = new GestureData();
                            g.time = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                            g.id = this.bodies[i].TrackingId;
                            g.type = this.gestureTrackers[i].gesture;
                            OnGestureChanged(g);
                        }

                        // check if a valid face is tracked in this face source
                        if (this.faceFrameSources[i].IsTrackingIdValid)
                        {
                            // check if we have valid face frame results
                            if (this.faceFrameResults[i] != null)
                            {
                                // draw face frame results
                                this.PrintFaceFrameResults(i, this.faceFrameResults[i]);
                            }
                        }
                        else
                        {
                            //Console.WriteLine(String.Format("{0}, {1}", i, this.bodies[i].IsTracked));
                            // check if the corresponding body is tracked 
                            if (this.bodies[i].IsTracked)
                            {
                                // update the face frame source to track this body
                                this.faceFrameSources[i].TrackingId = this.bodies[i].TrackingId;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws face frame results
        /// </summary>
        /// <param name="faceIndex">the index of the face frame corresponding to a specific body in the FOV</param>
        /// <param name="faceResult">container of all face frame results</param>
        /// <param name="drawingContext">drawing context to render to</param>
        private void PrintFaceFrameResults(int faceIndex, FaceFrameResult faceResult)
        {
            // draw the face bounding box
            var faceBoxSource = faceResult.FaceBoundingBoxInColorSpace;
            Rect faceBox = new Rect(faceBoxSource.Left, faceBoxSource.Top, faceBoxSource.Right - faceBoxSource.Left, faceBoxSource.Bottom - faceBoxSource.Top);

            FaceData faceData = new FaceData();
            faceData.id = faceResult.TrackingId;
            faceData.time = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            double height = MeasureHeight(this.bodies[faceIndex]);
            faceData.height = Math.Max(1.5, Math.Min(2.0, height));
            faceData.weight = faceData.height * faceData.height * 20;

            // extract each face property information and store it in faceText
            if (faceResult.FaceProperties != null)
            {
                faceData.happy = faceResult.FaceProperties[FaceProperty.Happy];
                faceData.wearingGlasses = faceResult.FaceProperties[FaceProperty.WearingGlasses];
            }

            // extract face rotation in degrees as Euler angles
            if (faceResult.FaceRotationQuaternion != null)
            {
                ExtractFaceRotationInDegrees(faceResult.FaceRotationQuaternion, out faceData.pitch, out faceData.yaw, out faceData.roll);
            }

            // render the face property and face rotation information
            Body body = this.bodies[faceIndex];
            if (body.IsTracked)
            {
                OnFaceChanged(faceData);
            }
        }

        /// <summary>
        /// Validates face bounding box and face points to be within screen space
        /// </summary>
        /// <param name="faceResult">the face frame result containing face box and points</param>
        /// <returns>success or failure</returns>
        private bool ValidateFaceBoxAndPoints(FaceFrameResult faceResult)
        {
            bool isFaceValid = faceResult != null;

            if (isFaceValid)
            {
                var faceBox = faceResult.FaceBoundingBoxInColorSpace;
                if (faceBox != null)
                {
                    // check if we have a valid rectangle within the bounds of the screen space
                    isFaceValid = (faceBox.Right - faceBox.Left) > 0 &&
                                  (faceBox.Bottom - faceBox.Top) > 0 &&
                                  faceBox.Right <= this.displayWidth &&
                                  faceBox.Bottom <= this.displayHeight;

                    if (isFaceValid)
                    {
                        var facePoints = faceResult.FacePointsInColorSpace;
                        if (facePoints != null)
                        {
                            foreach (Microsoft.Kinect.PointF pointF in facePoints.Values)
                            {
                                // check if we have a valid face point within the bounds of the screen space
                                bool isFacePointValid = pointF.X > 0.0f &&
                                                        pointF.Y > 0.0f &&
                                                        pointF.X < this.displayWidth &&
                                                        pointF.Y < this.displayHeight;

                                if (!isFacePointValid)
                                {
                                    isFaceValid = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return isFaceValid;
        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            if (this.kinectSensor != null)
            {
               Console.WriteLine(this.kinectSensor.IsAvailable ? "Sensor is available" : "Sensor is not available");
            }
        }
    }
}
