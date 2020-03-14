using System;
using System.Collections.Generic;

namespace veda.darknet.invoke
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
}
