using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    public class VectorLength : CommonF
    {
        private SVector arg;

        public VectorLength(SVector arg)
        {
            this.arg = arg;
            staticDelegate = p => Compute(arg.InvokeVec(p));
        }

        static double Compute(double[] p)
        {
            double sum = 0;
            for (int i = 0; i < p.Length; i++)
            {
                sum += p[i] * p[i];
            }
            return Math.Sqrt(sum);
        }

        private readonly Func<double[], double> staticDelegate;

        public override Func<double[], double> Invoke
        {
            get
            {
                return staticDelegate;
            }
        }

        public override bool IsZero()
        {
            return arg.IsZero();
        }

        public override string Print()
        {
            throw new NotImplementedException();
        }
        public override string PrintLatex()
        {
            throw new NotImplementedException();
        }

        public override Variable Search(string name)
        {
            Variable result = null;
            if (arg != null)
                for (int i = 0; i < arg.Count; i++)
                {
                    if (result == null)
                    {
                        result = arg[i].Search(name);
                    }
                    else
                    {
                        return result;
                    }
                }
            return result;
        }

        public override CommonF Clone()
        {
            return new VectorLength(arg.Clone());
        }
    }
}
