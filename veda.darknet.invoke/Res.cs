using System;
using System.Collections.Generic;

namespace veda.darknet.invoke
{
    public class DectationRes
    {
        public class BBox
        {
            public double x;
            public double y;
            public double w;
            public double h;
        }
        public int class_id;
        public string name;
        public BBox box;
        public double confidence;
        public List<string> lables;
        public List<double> probs;
    }
    
}
