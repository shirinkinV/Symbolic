using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    //strange unused class
    public class Curve : SVector
    {
        public Curve(List<CommonF> trajectory) : base(trajectory) { }

        public Func<double, double[]> Trajectory
        {
            get
            {
                return t => Func(new double[] { t });
            }
        }
    }
}
