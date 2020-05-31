using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    public class SVector : IFunction
    {


        private List<CommonF> components = new List<CommonF>();

        public SVector()
        {
            components = new List<CommonF>();
        }

        public SVector(List<CommonF> components)
        {
            this.components = components;
        }

        public SVector(List<DefinedCommonF> components)
        {
            this.components = new List<CommonF>();
            for (int i = 0; i < components.Count; i++)
            {
                this.components.Add(components[i]);
            }
        }

        public SVector Clone()
        {
            var newComponents = new List<CommonF>();
            foreach (var f in components)
            {
                newComponents.Add(f.Clone());
            }
            return new SVector(newComponents);
        }

        protected double[] Func(double[] p)
        {
            if (components.Count == 0)
            {
                return null;
            }
            double[] result = new double[components.Count];
            for (int i = 0; i < components.Count; i++)
            {
                result[i] = components[i].Invoke(p);
            }
            return result;
        }

        public Func<double[], double[]> InvokeVec
        {
            get
            {
                return Func;
            }
        }

        public Variable Search(string name)
        {
            Variable result = null;
            for (int i = 0; i < components.Count; i++)
            {
                if (result == null)
                {
                    result = components.ElementAt(i).Search(name);
                }
                else
                {
                    return result;
                }
            }
            return result;
        }

        public bool IsZero()
        {
            return components.All(it => it.IsZero());
        }

        #region Operators

        public static CommonF operator *(SVector left, SVector right)
        {
            if (left.Count != right.Count) throw new ArgumentException("vectors have different size");
            CommonF result = new Constant(0);

            for (int i = 0; i < left.Count; i++)
            {
                result += left[i] * right[i];
            }

            return result;
        }

        public static SVector operator +(SVector left, SVector right)
        {
            if (left.Count != right.Count) throw new ArgumentException("different vector sizes");
            var result = new SVector();
            for (int i = 0; i < left.Count; i++)
            {
                result[i] = left[i] + right[i];
            }
            return result;
        }
        public static SVector operator -(SVector left, SVector right)
        {
            if (left.Count != right.Count) throw new ArgumentException("different vector sizes");
            var result = new SVector();
            for (int i = 0; i < left.Count; i++)
            {
                result[i] = left[i] - right[i];
            }
            return result;
        }
        public static SVector operator -(SVector v)
        {
            var result = new SVector();
            for (int i = 0; i < v.Count; i++)
            {
                result[i] = -v[i];
            }
            return result;
        }
        public static SVector operator +(SVector v)
        {
            var result = new SVector();
            for (int i = 0; i < v.Count; i++)
            {
                result[i] = +v[i];
            }
            return result;
        }


        public CommonF this[int index]
        {
            get
            {
                if (index < components.Count)
                    return components[index];
                else
                    return new Constant(0);
            }
            set
            {
                while (index >= components.Count)
                {
                    components.Add(new Constant(0));
                }
                components[index] = value;
            }
        }

        #endregion

        public int Count { get { return components.Count; } }
    }
}
