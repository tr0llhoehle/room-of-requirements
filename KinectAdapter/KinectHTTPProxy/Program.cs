using System;

namespace KinectHTTPProxy
{
    class Program
    {

        static void Main(string[] args)
        {
            Kinect2HTTP k2http = new Kinect2HTTP();

            Console.ReadLine();

            k2http.Shutdown();
        }
    }
}
