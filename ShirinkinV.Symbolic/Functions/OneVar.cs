using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// One variable function
    /// </summary>
    public class OneVar : CommonF
    {
        /// <summary>
        /// argument of function - another function
        /// </summary>
        public CommonF arg;

        /// <summary>
        /// function delegate
        /// </summary>
        public Func<double, double> compute = p => Double.NaN;

        /// <summary>
        /// string function performance
        /// </summary>
        public string performance;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="function">function delegate</param>
        /// <param name="arg">argument of function</param>
        /// <param name="performance">string function performance, which has 'argument' instead of variable</param>
        public OneVar(Func<double, double> function, CommonF arg, string performance)
        {
            if (performance == "") throw new ArgumentException("empty string of performance");

            this.performance = performance;
            if (arg == null || function == null) throw new ArgumentException("function or argiment is null");
            compute = function;
            this.arg = arg;
            staticDelegate = p => compute(arg.Invoke(p));
        }

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
        /// Checking for identical zero
        /// </summary>
        /// <returns>logical value yes or no</returns>
        public override bool IsZero()
        {
            return false;
        }

        /// <summary>
        /// Print function formated C
        /// </summary>
        /// <returns>string containing function performance</returns>
        public override string Print()
        {
            string result = performance;
            int indexOfArgument = result.IndexOf("argument");
            result = result.Remove(indexOfArgument, 8);
            if (result[indexOfArgument - 1] == '(' && result[indexOfArgument] == ')')
                result = result.Insert(indexOfArgument, arg.Print());
            else
                result = result.Insert(indexOfArgument, "(" + arg.Print() + ")");
            return result;
        }

        public override string PrintLatex()
        {
            int indexOfArgument = performance.IndexOf("argument");
            var name = performance.Substring(0, indexOfArgument - 1);
            var afterName = $"\\left({arg.PrintLatex()}\\right)";
            if (name == "abs")
            {
                return $"\\left|{arg.PrintLatex()}\\right|";
            }
            else if (name == "acos")
            {
                //TODO!
                return $"";
            }
            else if (name == "acosh")
            {
                return $"";
            }
            else if (name == "asin")
            {
                return $"";
            }
            else if (name == "asinh")
            {
                return $"";
            }
            else if (name == "atan")
            {
                return $"";
            }
            else if (name == "atanh")
            {
                return $"";
            }
            else if (name == "cbrt")
            {
                return $"";
            }
            else if (name == "ceil")
            {
                return $"";
            }
            else if (name == "cos")
            {
                return $"\\cos{afterName}";
            }
            else if (name == "cosh")
            {
                return $"";
            }
            else if (name == "exp")
            {
                return $"";
            }
            else if (name == "floor")
            {
                return $"";
            }
            else if (name == "log")
            {
                return $"";
            }
            else if (name == "log10")
            {
                return $"";
            }
            else if (name == "signum")
            {
                return $"";
            }
            else if (name == "sin")
            {
                return $"\\sin{afterName}";
            }
            else if (name == "sinh")
            {
                return $"";
            }
            else if (name == "sqrt")
            {
                return $"";
            }
            else if (name == "tan")
            {
                return $"";
            }
            else if (name == "tanh")
            {
                return $"";
            }



            /*
            result = result.Remove(indexOfArgument, 8);
            if (result[indexOfArgument - 1] == '(' && result[indexOfArgument] == ')')
                result = result.Insert(indexOfArgument, "\\right").Insert(indexOfArgument, arg.PrintLatex()).Insert(indexOfArgument - 1, "\\left");
            else
                result = result.Insert(indexOfArgument, "\\left(" + arg.PrintLatex() + "\\right)");

            var name = performance.Substring(0, indexOfArgument - 1);
            performance.Remove(0, indexOfArgument - 1);

            if(name=="abs")
            */
            return "nothing";
        }

        /// <summary>
        /// Search variable by name
        /// </summary>
        /// <param name="name">variable name (including ' for derivatives)</param>
        /// <returns>object performing Variable</returns>
        public override Variable Search(string name)
        {
            return arg.Search(name);
        }

        public override CommonF Clone()
        {
            return new OneVar(compute, arg.Clone(), performance);
        }
    }
}
