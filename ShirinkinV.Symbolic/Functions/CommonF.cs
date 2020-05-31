using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Common performance of function of many variables. This is the base for all classes.
    /// </summary>
    public abstract class CommonF : IFunction
    {

        /// <summary>
        /// Add operator
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>New object typed Sum</returns>
        public static Sum operator +(CommonF left, CommonF right)
        {
            //new object
            Sum result = new Sum();
            //if left operand is sum of another functions
            if (left is Sum)
            {
                //add these functions to our sum
                result.operands = ((Sum)(+left)).operands;
                result.signs = ((Sum)(+left)).signs;
            }
            else
            {
                //if not than add the function to our sum
                result.operands.Add(left);
                result.signs.Add(true);
            }
            //same for right operand
            if (right is Sum)
            {
                result.operands.AddRange(((Sum)(+right)).operands);
                result.signs.AddRange(((Sum)(+right)).signs);
            }
            else
            {
                result.operands.Add(right);
                result.signs.Add(true);
            }
            return result;
        }

        public static CommonF operator +(CommonF f)
        {
            return f.Clone();
        }

        /// <summary>
        /// Sub operator 
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>new object typed Sum</returns>
        public static Sum operator -(CommonF left, CommonF right)
        {
            //very simple expression
            return left + (-right);
        }

        /// <summary>
        /// Neg operator
        /// </summary>
        /// <param name="operand">operand</param>
        /// <returns>object typed Sum with one term (or changes signs in operand if it is Sum)
        public static Sum operator -(CommonF operand)
        {
            Sum result = new Sum();
            if (operand is Sum)
            {
                result = (Sum)operand.Clone();
                for (int i = 0; i < result.signs.Count; i++)
                {
                    result.signs[i] = !result.signs[i];
                }
            }
            else
            {
                result.operands.Add(operand);
                result.signs.Add(false);
            }
            return result;
        }

        /// <summary>
        /// Mul operator
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>object typed Mul</returns>
        public static Mul operator *(CommonF left, CommonF right)
        {
            Mul result = new Mul();
            if (left is Mul)
            {
                result.operands = new List<CommonF>();
                foreach (var operand in ((Mul)left).operands)
                {
                    result.operands.Add(operand.Clone());
                }
                result.powers = new List<bool>();

                foreach (var power in ((Mul)left).powers)
                {
                    result.powers.Add(power);
                }
            }
            else
            {
                result.operands.Add(left);
                result.powers.Add(true);
            }
            if (right is Mul)
            {
                foreach (var operand in ((Mul)right).operands)
                {
                    result.operands.Add(operand.Clone());
                }
                foreach (var power in ((Mul)right).powers)
                {
                    result.powers.Add(power);
                }
            }
            else
            {
                result.operands.Add(right);
                result.powers.Add(true);
            }
            return result;
        }

        /// <summary>
        /// Div operator
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>object typed Mul</returns>
        public static Mul operator /(CommonF left, CommonF right)
        {
            Mul result = new Mul();
            if (left is Mul)
            {
                result.operands = ((Mul)left).operands;
                result.powers = ((Mul)left).powers;
            }
            else
            {
                result.operands.Add(left);
                result.powers.Add(true);
            }

            result.operands.Add(right);
            result.powers.Add(false);
            return result;
        }

        /// <summary>
        /// Pow operator
        /// </summary>
        /// <param name="base">power base</param>
        /// <param name="power">power</param>
        /// <returns>new object typed Pow</returns>
        public static Pow operator ^(CommonF @base, CommonF power)
        {
            Pow result = new Pow();
            result.RaiseTo(@base);
            result.RaiseTo(power);
            return result;
        }

        /// <summary>
        /// Main delegate of function
        /// </summary>
        public abstract Func<double[], double> Invoke { get; }

        /// <summary>
        /// Search variable by name
        /// </summary>
        /// <param name="name">variable name (including ' for derivatives)</param>
        /// <returns>object performing Variable</returns>
        public abstract Variable Search(string name);

        /// <summary>
        /// interface implementation
        /// </summary>
        public Func<double[], double[]> InvokeVec
        {
            get
            {
                return p => { arrayForResult[0] = Invoke(p); return arrayForResult; };
            }
        }

        //optimization 
        private double[] arrayForResult = new double[1];

        /// <summary>
        /// Check for identical zero
        /// </summary>
        /// <returns>logical value yes or no</returns>
        public abstract bool IsZero();

        public abstract CommonF Clone();

        /// <summary>
        /// Print function formated C
        /// </summary>
        /// <returns>string expression</returns>
        public abstract string Print();

        /// <summary>
        /// Print function formated LaTeX
        /// </summary>
        /// <returns></returns>
        public abstract string PrintLatex();

        /// <summary>
        /// Converting string to function
        /// </summary>
        /// <param name="s"> string must contain variables domain after ':' separated by ';', for example "x+y:x;y"</param>
        public static implicit operator CommonF(string s)
        {
            var indexOfBeginingVariablesDomain = s.IndexOf(':');
            var expression = s.Substring(0, indexOfBeginingVariablesDomain);
            var variableDomain = s.Substring(indexOfBeginingVariablesDomain + 1);
            string[] variables = null;
            if (variableDomain != "null")
                variables = variableDomain.Split(';');
            return Parser.ParseExpressionObject(expression, variables);
        }

        public static implicit operator CommonF(double d)
        {
            return new Constant(d);
        }
    }
}
