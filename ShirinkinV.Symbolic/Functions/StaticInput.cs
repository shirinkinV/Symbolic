using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    public class StaticInput
    {
        public static E _E
        {
            get
            {
                return new E();
            }
        }
        public static PI _PI
        {
            get
            {
                return new PI();
            }
        }
        public static OneVar _sin(CommonF arg)
        {
            return new OneVar(Math.Sin, arg, "sin(argument)");
        }

        public static OneVar _abs(CommonF arg)
        {
            return new OneVar(Math.Abs, arg, "abs(argument)");
        }
        public static OneVar _acos(CommonF arg)
        {
            return new OneVar(Math.Acos, arg, "acos(argument)");
        }
        public static OneVar _acosh(CommonF arg)
        {
            return new OneVar(MathNet.Numerics.Trig.Acosh, arg, "acosh(argument)");
        }
        public static OneVar _asin(CommonF arg)
        {
            return new OneVar(Math.Asin, arg, "asin(argument)");
        }
        public static OneVar _asinh(CommonF arg)
        {
            return new OneVar(MathNet.Numerics.Trig.Asinh, arg, "asinh(argument)");
        }
        public static OneVar _atan(CommonF arg)
        {
            return new OneVar(Math.Atan, arg, "atan(argument)");
        }
        public static OneVar _atanh(CommonF arg)
        {
            return new OneVar(MathNet.Numerics.Trig.Atanh, arg, "atanh(argument)");
        }
        public static OneVar _cbrt(CommonF arg)
        {
            return new OneVar(x => Math.Pow(x, 1 / 3), arg, "cbrt(argument)");
        }
        public static OneVar _ceil(CommonF arg)
        {
            return new OneVar(Math.Ceiling, arg, "ceil(argument)");
        }
        public static OneVar _cos(CommonF arg)
        {
            return new OneVar(Math.Cos, arg, "cos(argument)");
        }
        public static OneVar _cosh(CommonF arg)
        {
            return new OneVar(Math.Cosh, arg, "cosh(argument)");
        }
        public static OneVar _exp(CommonF arg)
        {
            return new OneVar(Math.Exp, arg, "exp(argument)");
        }
        public static OneVar _floor(CommonF arg)
        {
            return new OneVar(Math.Floor, arg, "floor(argument)");
        }
        public static OneVar _log(CommonF arg)
        {
            return new OneVar(Math.Log, arg, "log(argument)");
        }
        public static OneVar _log10(CommonF arg)
        {
            return new OneVar(Math.Log10, arg, "log10(argument)");
        }
        public static OneVar _signum(CommonF arg)
        {
            return new OneVar(x => Math.Sign(x), arg, "signum(argument)");
        }
        public static OneVar _sqrt(CommonF arg)
        {
            return new OneVar(Math.Sqrt, arg, "sqrt(argument)");
        }
        public static OneVar _sinh(CommonF arg)
        {
            return new OneVar(Math.Sinh, arg, "sinh(argument)");
        }
        public static OneVar _tan(CommonF arg)
        {
            return new OneVar(Math.Tan, arg, "tan(argument)");
        }
        public static OneVar _tanh(CommonF arg)
        {
            return new OneVar(Math.Tanh, arg, "tanh(argument)");
        }

        /// <summary>
        /// Domain of variables formated x;y;z (variables separated by ';')
        /// </summary>
        /// <param name="domain"></param>
        public static void setVariableDomain(string domain)
        {
            indices = new Dictionary<string, int>();
            var names = domain.Split(';');
            for (int i = 0; i < names.Length; i++)
            {
                indices[names[i]] = i;
            }
        }

        private static Dictionary<string, int> indices;

        public static Variable _var(string name)
        {
            return new Variable(indices[name], name);
        }

        public static Pow _pow(CommonF b, CommonF p)
        {
            return b ^ p;
        }
    }
}
