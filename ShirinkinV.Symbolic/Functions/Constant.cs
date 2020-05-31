using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Numeric constant
    /// </summary>
    public class Constant : OneVar
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">value of numeric constant</param>
        public Constant(double value) : base(_ => value, new Variable(-1, "x"), value >= 0 ? "" + value.ToString(CultureInfo.InvariantCulture) : "(" + value.ToString(CultureInfo.InvariantCulture) + ")")
        {
            this.value = value;
        }

        /// <summary>
        /// Print constant formated C
        /// </summary>
        /// <returns>string containing expression of a function</returns>
        public override string Print()
        {
            return performance;
        }

        public override string PrintLatex()
        {
            if (value == Math.PI || this is PI)
            {
                return "\\pi";
            }
            if (value == Math.E || this is E)
            {
                return "e";
            }
            if (Value - Math.Truncate(Value) == 0)
            {
                return ((int)Value).ToString();
            }
            return Value.ToString("F", CultureInfo.InvariantCulture);
        }

        //constant value
        private double value;

        /// <summary>
        /// Numeric constant value
        /// </summary>
        public double Value
        {
            private set
            {
                this.value = value;
                performance = value >= 0 ? "" + value.ToString(CultureInfo.InvariantCulture) : "(" + value.ToString(CultureInfo.InvariantCulture) + ")";
                compute = _ => value;
            }
            get { return value; }
        }

        public override CommonF Clone()
        {
            return new Constant(value);
        }

        /// <summary>
        /// Check for is a constant zero
        /// </summary>
        /// <returns>logical calue yes or no</returns>
        public override bool IsZero()
        {
            return Value == 0;
        }

        /// <summary>
        /// overrided add operator for constant
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns></returns>
        public static Constant operator +(Constant left, Constant right)
        {
            return new Constant(left.Value + right.Value);
        }
        /// <summary>
        /// overrided sub operator for constant
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns></returns>
        public static Constant operator -(Constant left, Constant right)
        {
            return new Constant(left.Value - right.Value);
        }
        /// <summary>
        /// overrided div operator for constant
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns></returns>
        public static Constant operator /(Constant left, Constant right)
        {
            return new Constant(left.Value / right.Value);
        }
        /// <summary>
        /// overrided mul operator for constant
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns></returns>
        public static Constant operator *(Constant left, Constant right)
        {
            return new Constant(left.Value * right.Value);
        }
        /// <summary>
        /// overrided neg operator for constant
        /// </summary>
        /// <param name="operand">operand</param>
        /// <returns></returns>
        public static Constant operator -(Constant operand)
        {
            return new Constant(-operand.value);
        }
        /// <summary>
        /// overrided pow operator for constant
        /// </summary>
        /// <param name="base">power base</param>
        /// <param name="power">power</param>
        /// <returns></returns>
        public static Constant operator ^(Constant @base, Constant power)
        {
            return new Constant(Math.Pow(@base.value, power.value));
        }

        public static implicit operator Constant(double c)
        {
            return new Constant(c);
        }
    }
}
