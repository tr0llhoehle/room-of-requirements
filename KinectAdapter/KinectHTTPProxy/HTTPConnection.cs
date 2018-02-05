using System;
using System.Web.Script.Serialization;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

using KinectFaceTracker;

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

        public HTTPConnection()
        {
            faceServer = new Uri(baseURL + "/face");
            gestureServer = new Uri(baseURL + "/gesture");
            colorServer = new Uri(baseURL + "/current_image?id={0}&time={1}");
            depthServer = new Uri(baseURL + "/depth?id={0}&time={1}");
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
                wc.UploadData(url, "POST", result);
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
            SendImage(bm, depthServer);
        }

        public void SendColorImage(ImageData bm)
        {
            SendImage(bm, colorServer);
        }

        public void SendData(FaceData data)
        {
            SendData(ser.Serialize(data), faceServer);
        }

        public void SendData(GestureData data)
        {
            SendData(ser.Serialize(data), gestureServer);
        }
    }
}
