﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Dark.net
{
    public class DectationRes
    {
        public class BBox
        {
            public double center_x;
            public double center_y;
            public double width;
            public double height;
        }
        public int class_id;
        public string name;
        public BBox relative_coordinates;
        public double confidence;
    }
    public class DectationResAry
    {
        public List<DectationRes> objects;
    }
    class Program
    {
        delegate void StringAct(string jsn);
        [DllImport("dark.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr ggCreateNetwork(string datacfg = "cfg/coco.data", string cfgfile = "cfg/yolov3.cfg", string weightfile = "yolov3.weights",
    int benchmark_layers = 0);

        [DllImport("dark.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern void gFreeNetwork(IntPtr info);

        [DllImport("dark.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern void gDetect(IntPtr info, byte[] imageBuffer, int imgLength, StringAct act,float thresh = 0.24f, float hier_thresh = 0.5f, int letter_box = 0);

            static void Main(string[] args)
        {
            
            
            var ptr = ggCreateNetwork();
            byte[] img = File.ReadAllBytes("dog.jpg");
            gDetect(ptr, img, img.Length, str=>
            {
                var rs = JsonConvert.DeserializeObject<DectationResAry>(str);
                Console.WriteLine(str);
            });
            gFreeNetwork(ptr);
            Console.WriteLine("Hello World!" + ptr);
        }
    }
}