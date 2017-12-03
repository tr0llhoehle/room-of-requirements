using System;
using System.Web.Script.Serialization;
using System.Net;

using KinectFaceTracker;

namespace KinectHTTPProxy
{
    class HTTPConnection
    {
        private Uri server = new Uri("http://127.0.0.1:3000/kinect");

        public void SendData(FaceData data)
        {
            var ser = new JavaScriptSerializer();
            var json = ser.Serialize(data);

            Console.WriteLine(json);

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.UploadStringAsync(server, "POST", json);
            }
        }
    }
}
