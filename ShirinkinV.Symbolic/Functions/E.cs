using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Constant E
    /// </summary>
    public class E : Constant
    {
        /// <summary>
        /// Simple constructor
        /// </summary>
        public E() : base(Math.E)
        {
            performance = "E";
        }
        public override CommonF Clone()
        {
            return new E();
        }
    }
}
