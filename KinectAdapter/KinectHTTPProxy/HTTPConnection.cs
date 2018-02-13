using System;
using System.Web.Script.Serialization;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

using KinectFaceTracker;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace KinectHTTPProxy
{
    class HTTPConnection
    {
        private String baseURL = "http://127.0.0.1:3000";
        private Uri faceServer;
        private Uri gestureServer;
        private Uri depthServer;
        private Uri colorServer;

        private JavaScriptSerializer ser = new JavaScriptSerializer();

        bool running = true;
        private BlockingCollection<ImageData> image_queue = new BlockingCollection<ImageData>();
        private BlockingCollection<ImageData> depth_queue = new BlockingCollection<ImageData>();
        private BlockingCollection<FaceData> face_queue = new BlockingCollection<FaceData>();
        private BlockingCollection<GestureData> gesture_queue = new BlockingCollection<GestureData>();

        public HTTPConnection()
        {
            faceServer = new Uri(baseURL + "/face");
            gestureServer = new Uri(baseURL + "/gesture");
            colorServer = new Uri(baseURL + "/current_image?id={0}&time={1}");
            depthServer = new Uri(baseURL + "/depth?id={0}&time={1}");
        }

        public Task Connect()
        {
            var image_task = Task.Run(() => {
                while (running)
                {
                    ImageData image = image_queue.Take();
                    SendImage(image, colorServer);
                }
            });

            var face_task = Task.Run(() => {
                while (running)
                {
                    FaceData face = face_queue.Take();
                    SendData(ser.Serialize(face), faceServer);
                }
            });

            var gesture_task = Task.Run(() => {
                while (running)
                {
                    GestureData gesture = gesture_queue.Take();
                    SendData(ser.Serialize(gesture), gestureServer);
                }
            });

            return Task.Run(() => {
                Task.WaitAll(new Task[] { image_task, face_task, gesture_task });
            });
        }

        public void Shutdown()
        {
            running = false;
        }

        private void SendImage(ImageData bm, Uri url)
        {
            byte[] result;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                if (bm.image.PixelFormat == PixelFormat.Format16bppGrayScale)
                {
                    // FIXME: gray scale conversion does not work
                    return;
                    /*
                    
                    Bitmap rgb_bm = new Bitmap(bm.Width, bm.Height,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                    using (Graphics gr = Graphics.FromImage(rgb_bm))
                    {
                        gr.DrawImage(bm, new Rectangle(0, 0, rgb_bm.Width, rgb_bm.Height));
                    }

                    bm = rgb_bm;
                    */
                }

                bm.image.Save(stream, ImageFormat.Png);

                result = stream.ToArray();
            }

            url = new Uri(string.Format(url.ToString(), bm.id, bm.time));

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "image/png";
                wc.UploadDataAsync(url, "POST", result);
            }
        }

        private void SendData(String json, Uri url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.UploadStringAsync(url, "POST", json);
            }
        }

        public void SendDepthImage(ImageData bm)
        {
            depth_queue.Add(bm);
        }

        public void SendColorImage(ImageData bm)
        {
            image_queue.Add(bm);
        }

        public void SendData(FaceData data)
        {
            face_queue.Add(data);
        }

        public void SendData(GestureData data)
        {
            gesture_queue.Add(data);
        }
    }
}
