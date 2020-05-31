using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Multiplication or devision of another functions
    /// </summary>
    public class Mul : CommonF
    {
        /// <summary>
        /// list of operands
        /// </summary>
        public List<CommonF> operands;
        /// <summary>
        /// list of powers. Powers can be +1 or -1. boolean value.
        /// </summary>
        public List<bool> powers;

        /// <summary>
        /// Constructor
        /// </summary>
        public Mul()
        {
            operands = new List<CommonF>();
            powers = new List<bool>();
        }

        /// <summary>
        /// Constructor with source data
        /// </summary>
        /// <param name="operands">list of operands</param>
        /// <param name="powers">list of powers +1 or -1</param>
        public Mul(List<CommonF> operands, List<bool> powers)
        {
            this.operands = operands;
            this.powers = powers;
        }

        private double Compute(double[] p)
        {
            double result = operands[0].Invoke(p);
            for (int i = 1; i < operands.Count; i++)
            {
                if (powers[i])
                {
                    result *= operands[i].Invoke(p);
                }
                else
                {
                    result /= operands[i].Invoke(p);
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
        /// Checking for idential zero 
        /// </summary>
        /// <returns>logical value yes or no</returns>
        public override bool IsZero()
        {
            if (operands.Count == 0) return true;
            for (int i = 0; i < operands.Count; i++)
            {
                if (operands[i].IsZero()) return true;
            }
            return false;
        }


        /// <summary>
        /// Print function formated C
        /// </summary>
        /// <returns>string containing function performance</returns>
        public override string Print()
        {
            string result = "";
            for (int i = 0; i < operands.Count; i++)
            {
                if (i != 0) result += powers[i] ? "*" : "/";
                if (operands[i] is Sum)
                {
                    result += "(" + operands[i].Print() + ")";
                }
                else
                {
                    string factor = operands[i].Print();
                    if (factor[0] != '-')
                        result += factor;
                    else
                        result += "(" + factor + ")";
                }
            }
            return result;
        }

        public override string PrintLatex()
        {
            string result = "";
            for (int i = 0; i < operands.Count; i++)
            {
                if (i != 0) result += powers[i] ? "\\cdot " : "/";
                if (operands[i] is Sum)
                {
                    result += "\\left(" + operands[i].PrintLatex() + "\\right)";
                }
                else
                {
                    string factor = operands[i].PrintLatex();
                    if (factor[0] != '-')
                        result += factor;
                    else
                        result += "\\left(" + factor + "\\right)";
                }
            }
            return result;
        }

        public override CommonF Clone()
        {
            var newOperands = new List<CommonF>();
            var newPowers = new List<bool>();
            for (int i = 0; i < operands.Count; i++)
            {
                newOperands.Add(operands[i].Clone());
                newPowers.Add(powers[i]);
            }
            return new Mul(newOperands, newPowers);
        }
    }
}
