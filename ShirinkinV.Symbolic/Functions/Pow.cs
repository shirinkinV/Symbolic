using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Power
    /// </summary>
    public class Pow : CommonF
    {
        /// <summary>
        /// sequence of base and power, for example: x^x^x
        /// </summary>
        public List<CommonF> baseAndPower;

        /// <summary>
        /// constructor
        /// </summary>
        public Pow()
        {
            baseAndPower = new List<CommonF>();
            staticDelegate = p => Compute(p);
        }

        /// <summary>
        /// constructor with source data
        /// </summary>
        /// <param name="baseAndPower">sequence of base and power, for example: x^x^x</param>
        public Pow(List<CommonF> baseAndPower)
        {
            this.baseAndPower = baseAndPower;
            staticDelegate = p => Compute(p);
        }

        /// <summary>
        /// Computing power
        /// </summary>
        /// <param name="p">variables values array</param>
        /// <returns></returns>
        private double Compute(double[] p)
        {
            if (baseAndPower.Count == 1) return baseAndPower[0].Invoke(p);
            else
            {
                double result = baseAndPower[baseAndPower.Count - 1].Invoke(p);
                for (int i = baseAndPower.Count - 2; i >= 0; i--)
                {
                    result = Math.Pow(baseAndPower[i].Invoke(p), result);
                }
                return result;
            }
        }

        //for optimization
        private readonly Func<double[], double> staticDelegate;
        /// <summary>
        /// Main function delegate
        /// </summary>
        public override Func<double[], double> Invoke
        {
            get
            {
                return staticDelegate;
            }
        }
        /// <summary>
        /// Search variable by name
        /// </summary>
        /// <param name="name">variable name (including ' for derivatives)</param>
        /// <returns>object performing Variable</returns>
        public override Variable Search(string name)
        {
            Variable result = null;
            for (int i = 0; i < baseAndPower.Count; i++)
            {
                if (result == null)
                {
                    result = baseAndPower.ElementAt(i).Search(name);
                }
                else
                {
                    return result;
                }
            }
            return result;
        }
        /// <summary>
        /// Check for identical zero
        /// </summary>
        /// <returns>logical value yes or no</returns>
        public override bool IsZero()
        {
            if (baseAndPower.Count == 0) return true;
            return baseAndPower[0].IsZero();
        }
        /// <summary>
        /// Print function formated C
        /// </summary>
        /// <returns>string expression</returns>
        public override string Print()
        {
            string result = "";
            if (baseAndPower[0] is Mul || baseAndPower[0] is Sum)
                result += "(" + baseAndPower[0].Print() + ")";
            else
                result += baseAndPower[0].Print();
            for (int i = 1; i < baseAndPower.Count; i++)
            {
                if (i != 0) result += "^";
                if (baseAndPower[i] is Mul || baseAndPower[i] is Sum)
                {
                    result += "(" + baseAndPower[i].Print() + ")";
                }
                else
                {
                    result += baseAndPower[i].Print();
                }
            }
            return result;
        }

        public override string PrintLatex()
        {
            string result = "";
            if (baseAndPower[0] is Mul || baseAndPower[0] is Sum)
                result += "\\left(" + baseAndPower[0].PrintLatex() + "\\right)";
            else
                result += baseAndPower[0].PrintLatex();
            for (int i = 1; i < baseAndPower.Count; i++)
            {
                if (i != 0) result += "^";

                result += "{" + baseAndPower[i].PrintLatex() + "}";

            }
            return result;
        }

        /// <summary>
        /// to power
        /// </summary>
        /// <param name="power">power</param>
        public void RaiseTo(CommonF power)
        {
            if (!(power is Pow))
                baseAndPower.Add(power);
            else
                baseAndPower.AddRange(((Pow)power).baseAndPower);
        }

        public override CommonF Clone()
        {
            var newBaseAndPower = new List<CommonF>();
            foreach (var f in baseAndPower)
            {
                newBaseAndPower.Add(f.Clone());
            }
            return new Pow(newBaseAndPower);
        }
    }
}
