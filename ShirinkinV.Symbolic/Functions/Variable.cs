using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    /// <summary>
    /// Переменная
    /// </summary>
    public class Variable : CommonF
    {
        /// <summary>
        /// variable index in expression
        /// </summary>
        public int index;
        /// <summary>
        /// variable name. may contain ' for derivatives
        /// </summary>
        public string name;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">variable index in expression</param>
        /// <param name="name">variable name. may contain ' for derivatives</param>
        public Variable(int index, string name)
        {
            this.index = index;
            this.name = name;
            thisValueDelegate = p => p[index];
        }

        //for optimization
        private readonly Func<double[], double> zeroDelegate = x => 0;
        private readonly Func<double[], double> thisValueDelegate;

        /// <summary>
        /// Main function delegate
        /// </summary>
        public override Func<double[], double> Invoke
        {
            get
            {
                if (index == -1) return zeroDelegate;

                return thisValueDelegate;
            }
        }

        /// <summary>
        /// Check for identical zero
        /// </summary>
        /// <returns>logical value yes or no</returns>
        public override bool IsZero()
        {
            return false;
        }

        /// <summary>
        /// Print function formated C
        /// </summary>
        /// <returns>string expression</returns>
        public override string Print()
        {
            return name;
        }

        public override string PrintLatex()
        {
            var result = name + "";
            var indexofPhi = result.IndexOf("phi");
            if (indexofPhi != -1)
            {
                result = result.Remove(indexofPhi, 3);
                result = result.Insert(indexofPhi, "\\varphi");

                if (result.IndexOf("'''") == -1)
                {
                    if (result.IndexOf("''") == result.Length - 2)
                    {
                        result = result.Remove(result.Length - 2, 2);
                        //{\ddot{
                        //\varphi
                        //}}
                        result = result.Insert(indexofPhi + 7, "}}");
                        result = result.Insert(indexofPhi, "{\\ddot{");
                    }
                    else if (result.IndexOf("'") == result.Length - 1)
                    {
                        result = result.Remove(result.Length - 1, 1);
                        result = result.Insert(indexofPhi + 7, "}}");
                        result = result.Insert(indexofPhi, "{\\dot{");
                    }
                    var indexOfBottomLine = result.IndexOf('_');
                    if (indexOfBottomLine == result.Length - 1) { }
                    else
                    {
                        result = result.Insert(result.Length, "}");
                        result = result.Insert(indexOfBottomLine + 1, "{");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Search variable by name
        /// </summary>
        /// <param name="name">variable name (including ' for derivatives)</param>
        /// <returns>object performing Variable</returns>
        public override Variable Search(string name)
        {
            if (this.name == name)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public override CommonF Clone()
        {
            return new Variable(index, name);
        }
    }
}
