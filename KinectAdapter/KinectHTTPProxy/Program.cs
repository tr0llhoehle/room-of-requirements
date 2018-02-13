using System;

using System.Windows.Forms;

namespace KinectHTTPProxy
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Kinect2HTTP k2http = new Kinect2HTTP();

            var t = k2http.Connect();

            if (k2http.DebugWindow != null)
            {
                Application.Run(k2http.DebugWindow);
            }

            t.Wait();

            k2http.Shutdown();
        }
    }
}
