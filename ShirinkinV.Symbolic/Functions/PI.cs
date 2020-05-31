using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Constant PI
    /// </summary>
    public class PI : Constant
    {
        /// <summary>
        /// Simple constructor
        /// </summary>
        public PI() : base(Math.PI)
        {
            performance = "PI";
        }
        public override CommonF Clone()
        {
            return new PI();
        }
    }
}
