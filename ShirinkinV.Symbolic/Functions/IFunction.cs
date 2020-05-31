using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Main interface. Now not using.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// main multivariable-vector-function delegate
        /// </summary>
        Func<double[], double[]> InvokeVec { get; }

        /// <summary>
        /// Search variable by name
        /// </summary>
        /// <param name="name">variable name (including ' for derivatives)</param>
        /// <returns>object performing new Variable</returns>
        Variable Search(string name);
    }
}
