using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;

namespace veda.darknet.invoke
{
    public class Core
    {
        delegate void StringAct(string jsn);
        [DllImport("dark.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr ggCreateNetwork(string datacfg = "cfg/coco.data", string cfgfile = "cfg/yolov3.cfg", string weightfile = "yolov3.weights",
    int benchmark_layers = 0);

        [DllImport("dark.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void gFreeNetwork(IntPtr info);

        [DllImport("dark.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern void gDetect(IntPtr info, byte[] imageBuffer, int imgLength, StringAct act, float thresh = 0.24f, float hier_thresh = 0.5f, int letter_box = 0);

        public static DectationResAry Detect(IntPtr net, byte[] imgBuf, float threshold)
        {
            DectationResAry res = null;
            gDetect(net, imgBuf, imgBuf.Length, str =>
            {
                res = JsonConvert.DeserializeObject<DectationResAry>(str);
            });
            return res;
        }
    }
}
