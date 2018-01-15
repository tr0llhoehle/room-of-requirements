using System;
using System.Web.Script.Serialization;
using System.Net;

using KinectFaceTracker;

namespace KinectHTTPProxy
{
    class HTTPConnection
    {
        private String baseURL = "http://127.0.0.1:3000";
        private Uri faceServer;
        private Uri gestureServer;

        private JavaScriptSerializer ser = new JavaScriptSerializer();

        public HTTPConnection()
        {
            faceServer = new Uri(baseURL + "/face");
            gestureServer = new Uri(baseURL + "/gesture");
        }

        private void SendData(String json, Uri url)
        {
            Console.WriteLine(json + "->" + url.ToString());

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.UploadStringAsync(url, "POST", json);
            }
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
