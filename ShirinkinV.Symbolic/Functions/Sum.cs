using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Sum of other functions
    /// </summary>
    public class Sum : CommonF
    {
        /// <summary>
        /// operands
        /// </summary>
        public List<CommonF> operands;
        /// <summary>
        /// sign + or - for each operand
        /// </summary>
        public List<bool> signs;

        /// <summary>
        /// constructor
        /// </summary>
        public Sum()
        {
            operands = new List<CommonF>();
            signs = new List<bool>();
        }

        /// <summary>
        /// constructor with source data
        /// </summary>
        /// <param name="operands">operands</param>
        /// <param name="signs">signs + - for each operand</param>
        public Sum(List<CommonF> operands, List<bool> signs)
        {
            this.operands = operands;
            this.signs = signs;
        }

        /// <summary>
        /// evaluate sum
        /// </summary>
        /// <param name="p">arrau of variables values</param>
        /// <returns>sum value</returns>
        private double Compute(double[] p)
        {
            double result = 0;
            for (int i = 0; i < operands.Count; i++)
            {
                if (signs[i])
                {
                    result += operands[i].Invoke(p);
                }
                else
                {
                    result -= operands[i].Invoke(p);
                }
            }
            return result;
        }
        /// <summary>
        /// Main function delegate
        /// </summary>
        public override Func<double[], double> Invoke
        {
            get
            {
                return Compute;
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

            for (int i = 0; i < operands.Count; i++)
            {
                if (result == null)
                {
                    result = operands.ElementAt(i).Search(name);
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
            if (operands.Count == 0) return true;
            else return operands.All(it => it.IsZero());
        }
        /// <summary>
        /// Print function formated C
        /// </summary>
        /// <returns>string expression</returns>
        public override string Print()
        {
            string result = "";
            for (int i = 0; i < operands.Count; i++)
            {
                if (i == 0)
                {
                    if (!signs[0])
                    {
                        result += "-";
                    }
                }
                else
                {
                    result += signs[i] ? "+" : "-";
                }
                if (!signs[i])
                {
                    if (operands[i] is Sum)
                    {
                        result += "(" + operands[i].Print() + ")";
                    }
                    else
                    {
                        string term = operands[i].Print();
                        if (term[0] != '-')
                            result += term;
                        else
                            result += "(" + term + ")";
                    }
                }
                else
                {
                    string term = operands[i].Print();
                    if (term[0] != '-')
                        result += term;
                    else
                        result += "(" + term + ")";
                }
            }
            return result;
        }

        public override string PrintLatex()
        {
            string result = "";
            for (int i = 0; i < operands.Count; i++)
            {
                if (i == 0)
                {
                    if (!signs[0])
                    {
                        result += "-";
                    }
                }
                else
                {
                    result += signs[i] ? "+" : "-";
                }
                if (!signs[i])
                {
                    if (operands[i] is Sum)
                    {
                        result += "\\left(" + operands[i].PrintLatex() + "\\right)";
                    }
                    else
                    {
                        string term = operands[i].PrintLatex();
                        if (term[0] != '-')
                            result += term;
                        else
                            result += "\\left(" + term + "\\right)";
                    }
                }
                else
                {
                    string term = operands[i].PrintLatex();
                    if (term[0] != '-')
                        result += term;
                    else
                        result += "\\left(" + term + "\\right)";
                }
            }
            return result;
        }
        public override CommonF Clone()
        {
            var newOperands = new List<CommonF>();
            var newSigns = new List<bool>();
            for (int i = 0; i < operands.Count; i++)
            {
                newOperands.Add(operands[i].Clone());
                newSigns.Add(signs[i]);
            }
            return new Sum(newOperands, newSigns);
        }
    }
}
