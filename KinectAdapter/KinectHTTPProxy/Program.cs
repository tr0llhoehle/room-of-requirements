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

            if (k2http.DebugWindow != null)
            {
                Application.Run(k2http.DebugWindow);
            }

            k2http.Shutdown();
        }
    }
}
