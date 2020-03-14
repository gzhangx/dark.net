using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using veda.darknet.invoke;

namespace Dark.net
{

    class Program
    {

        static void Main(string[] args)
        {

            IntPtr net = Core.ggCreateNetwork();
            byte[] img = File.ReadAllBytes("dog.jpg");
            var res = Core.Detect(net, img, 0.8f);
            Console.WriteLine(res);
            Core.gFreeNetwork(net);
        }
    }
}
