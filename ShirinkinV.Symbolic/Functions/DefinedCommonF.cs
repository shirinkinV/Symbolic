using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Multivariable function defined non-textual way. Function has not analitic performance.
    /// </summary>
    public class DefinedCommonF : CommonF
    {
        protected Func<double[], double> function;
        protected string performance;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="function">function delegate</param>
        /// <param name="performance">users string-performance of function</param>
        public DefinedCommonF(Func<double[], double> function, string performance)
        {
            this.performance = performance;
            this.function = function;
        }

        /// <summary>
        /// Main function delegate
        /// </summary>
        public override Func<double[], double> Invoke
        {
            get
            {
                return function;
            }
        }

        public override CommonF Clone()
        {
            return new DefinedCommonF(function, performance);
        }

        /// <summary>
        /// Checking for a idential zero
        /// </summary>
        /// <returns>logical value yes or no</returns>
        public override bool IsZero()
        {
            //we think that delegate is not zero
            return false;
        }

        /// <summary>
        /// Print users performance
        /// </summary>
        /// <returns>string containing function performance</returns>
        public override string Print()
        {
            return performance;
        }

        public override string PrintLatex()
        {
            return performance;
        }

        /// <summary>
        /// Search variable by name
        /// </summary>
        /// <param name="name">variable name (including ' for derivatives)</param>
        /// <returns>allways returns null, because function has not analitic performance</returns>
        public override Variable Search(string name)
        {
            return null;
        }
    }
}
