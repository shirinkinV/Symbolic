using System;
using System.Collections.Generic;
using System.Linq;

namespace ShirinkinV.Symbolic
{
    public class Derivative
    {
        private string expression;
        private string variable;
        private Dictionary<string, string> elementalDerivatives = new Dictionary<string, string>();
        private Dictionary<string, string> exchanges;
        private bool fullDerivative;

        private Derivative(string expr, string variable, Dictionary<string, string> exchanges, bool fullDerivative)
        {
            this.fullDerivative = fullDerivative;
            if (exchanges == null) exchanges = new Dictionary<string, string>();
            this.exchanges = exchanges;
            expression = expr;
            this.variable = variable;
            InitElementalDerivatives();
            DeleteWhiteSpace();
            if (expression.IndexOf('+') == 0)
            {
                expression = expression.Remove(0, 1);
            }

            //replacement of brackets and interiors of brackets
            int indexOfOpen = -1;
            while (Has("(", indexOfOpen + 1))
            {
                indexOfOpen = expression.IndexOf('(', indexOfOpen + 1);
                int indexOfClose = -1;
                int opened = 1;
                for (int i = indexOfOpen + 1; i < expression.Length; i++)
                {
                    if (expression[i] == '(')
                        opened++;
                    if (expression[i] == ')')
                        opened--;
                    if (opened == 0)
                    {
                        indexOfClose = i;
                        break;
                    }
                }
                if (indexOfClose == -1)
                {
                    throw new Exception(") expected");
                }
                string newName = "";
                string exchanged = "";
                if (indexOfOpen != 0 && char.IsLetter(expression[indexOfOpen - 1]))
                {
                    string funcName = GetNameFromLastSymbolIndex(indexOfOpen - 1);
                    if (!elementalDerivatives.ContainsKey(funcName))
                    {
                        throw new Exception("not exists function " + funcName);
                    }

                    if (indexOfOpen == (expression[0] == '-' ? funcName.Length + 1 : funcName.Length))
                    {

                        exchanged = expression.Substring(indexOfOpen + 1, indexOfClose - indexOfOpen - 1);
                        if (!IsChangedName(exchanged))
                        {
                            expression = expression.Remove(indexOfOpen + 1, indexOfClose - indexOfOpen - 1);
                            newName = "u" + exchanges.Count;
                            expression = expression.Insert(indexOfOpen + 1, newName);
                            exchanges[newName] = exchanged;
                        }
                    }
                    else
                    {
                        exchanged = expression.Substring(indexOfOpen - funcName.Length, indexOfClose - (indexOfOpen - funcName.Length) + 1);
                        if (!IsChangedName(exchanged))
                        {
                            expression = expression.Remove(indexOfOpen - funcName.Length, indexOfClose - (indexOfOpen - funcName.Length) + 1);
                            newName = "u" + exchanges.Count;
                            expression = expression.Insert(indexOfOpen - funcName.Length, newName);
                            exchanges[newName] = exchanged;
                            indexOfOpen -= funcName.Length;
                        }
                    }
                }
                else
                {
                    exchanged = expression.Substring(indexOfOpen + 1, indexOfClose - indexOfOpen - 1);
                    if (!IsChangedName(exchanged))
                    {
                        expression = expression.Remove(indexOfOpen, indexOfClose - indexOfOpen + 1);
                        newName = "u" + exchanges.Count;
                        expression = expression.Insert(indexOfOpen, newName);
                        exchanges[newName] = exchanged;
                    }
                    else
                    {
                        expression = expression.Remove(indexOfClose, 1);
                        expression = expression.Remove(indexOfOpen, 1);
                    }
                }

            }
            //replacement of terms
            int adopIndex = Math.Max(expression.LastIndexOf('-'), expression.LastIndexOf('+'));
            if (adopIndex >= 1)
            {

                string exchanged1 = expression.Substring(0, adopIndex);
                if (adopIndex + 1 == expression.Length)
                {
                    throw new Exception("expression expected after + or -");
                }
                string exchanged2 = expression.Substring(adopIndex + 1);
                if (!IsChangedName(exchanged2))
                {
                    string name = "u" + exchanges.Count;
                    expression = expression.Remove(adopIndex + 1);
                    expression += name;
                    exchanges[name] = exchanged2;
                }
                if (!IsChangedName(exchanged1))
                {
                    string name = "u" + exchanges.Count;
                    expression = expression.Remove(0, adopIndex);
                    expression = expression.Insert(0, name);
                    exchanges[name] = exchanged1;
                }
            }
            //replacement of factors and divisors
            int muldivIndex = Math.Max(expression.LastIndexOf('*'), expression.LastIndexOf('/'));
            if (muldivIndex == 0)
            {
                throw new Exception("expression excepted for * or /");
            }
            if (muldivIndex >= 1)
            {
                string exchanged1 = expression.Substring(0, muldivIndex);
                if (muldivIndex + 1 == expression.Length)
                {
                    throw new Exception("expression expected after * or /");
                }
                string exchanged2 = expression.Substring(muldivIndex + 1);
                if (!IsChangedName(exchanged2))
                {
                    string name = "u" + exchanges.Count;
                    expression = expression.Remove(muldivIndex + 1);
                    expression += name;
                    exchanges[name] = exchanged2;
                }
                if (!IsChangedName(exchanged1))
                {
                    string name = "u" + exchanges.Count;
                    expression = expression.Remove(0, muldivIndex);
                    expression = expression.Insert(0, name);
                    exchanges[name] = exchanged1;
                }
            }
            //replacement of powers
            int caretIndex = expression.IndexOf('^');
            if (caretIndex == 0)
            {
                throw new Exception("expression excepted for ^");
            }
            if (caretIndex > 0)
            {
                if (expression[0] == '-')
                {
                    string exchanged = expression.Substring(1);
                    string name = "u" + exchanges.Count;
                    exchanges[name] = exchanged;
                    expression = "-" + name;
                }
                else
                {
                    string exchanged1 = expression.Substring(0, caretIndex);
                    if (caretIndex + 1 == expression.Length)
                    {
                        throw new Exception("expression expected after ^");
                    }
                    string exchanged2 = expression.Substring(caretIndex + 1);
                    if (!IsChangedName(exchanged2))
                    {
                        string name = "u" + exchanges.Count;
                        expression = expression.Remove(caretIndex + 1);
                        expression += name;
                        exchanges[name] = exchanged2;
                    }
                    if (!IsChangedName(exchanged1))
                    {
                        string name = "u" + exchanges.Count;
                        expression = expression.Remove(0, caretIndex);
                        expression = expression.Insert(0, name);
                        exchanges[name] = exchanged1;
                    }
                }
            }
            //expression is simple appearance
        }


        private void InitElementalDerivatives()
        {
            elementalDerivatives["abs"] = "signum(param)";
            elementalDerivatives["acos"] = "(-1/sqrt(1-(param)^2))";
            elementalDerivatives["acosh"] = "(1/sqrt((param)^2-1))";
            elementalDerivatives["asin"] = "(1/sqrt(1-(param)^2))";
            elementalDerivatives["asinh"] = "(1/sqrt(1+(param)^2))";
            elementalDerivatives["atan"] = "(1/((param)^2+1))";
            elementalDerivatives["atanh"] = "(1/(1-(param)^2))";
            elementalDerivatives["cbrt"] = "((param)^(-2/3)/3)";
            elementalDerivatives["ceil"] = "0";
            elementalDerivatives["cos"] = "(-sin(param))";
            elementalDerivatives["cosh"] = "sinh(param)";
            elementalDerivatives["exp"] = "exp(param)";
            elementalDerivatives["floor"] = "0";
            elementalDerivatives["log"] = "(param)^(-1)";
            elementalDerivatives["log10"] = "((param)^(-1)/log(10))";
            elementalDerivatives["signum"] = "0";
            elementalDerivatives["sin"] = "cos(param)";
            elementalDerivatives["sinh"] = "cosh(param)";
            elementalDerivatives["sqrt"] = "((param)^(-1/2)/2)";
            elementalDerivatives["tan"] = "cos(param)^(-2)";
            elementalDerivatives["tanh"] = "cosh(param)^(-2)";
            elementalDerivatives["rtd"] = "0";
            elementalDerivatives["dtr"] = "0";
        }


        private string ComputeDerivative()
        {
            int indexOfOpen = 0;
            int indexOfClose = 0;
            string derivative = "";
            int indexOfArgument = 0;
            string paramNoRevEx = "";
            string paramRevEx = "";
            int indexOfOperator = 0;
            string operand1 = "";
            string operand2 = "";
            int type = GetTypeOfExpression();
            switch (type)
            {
                case 1: return "(-" + GetDerivative(exchanges[expression.Substring(1)], variable, exchanges, fullDerivative) + ")";
                case 2:
                    if (variable == "t" && variable != expression.Substring(1))
                    {
                        if (fullDerivative)
                            return "(" + expression + "')";
                        else
                            return "0";
                    }
                    else
                    {
                        return variable == expression.Substring(1) ? "(-1)" : "0";
                    }
                case 3: return "0";
                case 4:
                    indexOfOpen = expression.IndexOf('(');
                    indexOfClose = expression.IndexOf(')');
                    derivative = elementalDerivatives[expression.Substring(1, indexOfOpen - 1)];
                    indexOfArgument = derivative.IndexOf("param");
                    if (indexOfArgument != -1)
                    {
                        derivative = derivative.Remove(indexOfArgument, 5);
                        paramRevEx = ReverseExchange(expression.Substring(indexOfOpen + 1, indexOfClose - indexOfOpen - 1));
                        paramNoRevEx = exchanges[expression.Substring(indexOfOpen + 1, indexOfClose - indexOfOpen - 1)];
                        derivative = derivative.Insert(indexOfArgument, paramRevEx);
                        return "(-" + derivative + "*" + GetDerivative(paramNoRevEx, variable, exchanges, fullDerivative) + ")";
                    }
                    else
                    {
                        return "(-" + derivative + ")";
                    }
                case 5: return GetDerivative(exchanges[expression], variable, exchanges, fullDerivative);
                case 6:
                    if (variable == "t" && variable != expression)
                    {
                        if (fullDerivative)
                            return "(" + expression + "')";
                        else
                            return "0";
                    }
                    else
                    {
                        return variable == expression ? "1" : "0";
                    }
                case 7: return "0";
                case 8:
                    indexOfOpen = expression.IndexOf('(');
                    indexOfClose = expression.IndexOf(')');
                    derivative = elementalDerivatives[expression.Substring(0, indexOfOpen)];
                    indexOfArgument = derivative.IndexOf("param");
                    if (indexOfArgument != -1)
                    {
                        derivative = derivative.Remove(indexOfArgument, 5);
                        paramRevEx = ReverseExchange(expression.Substring(indexOfOpen + 1, indexOfClose - indexOfOpen - 1));
                        paramNoRevEx = exchanges[expression.Substring(indexOfOpen + 1, indexOfClose - indexOfOpen - 1)];
                        derivative = derivative.Insert(indexOfArgument, paramRevEx);
                        return "(" + derivative + "*" + GetDerivative(paramNoRevEx, variable, exchanges, fullDerivative) + ")";
                    }
                    else
                    {
                        return "(" + derivative + ")";
                    }
                case 9:
                    indexOfOperator = expression.IndexOf('+');
                    return "(" + GetDerivative(exchanges[expression.Substring(0, indexOfOperator)], variable, exchanges, fullDerivative) + "+" + GetDerivative(exchanges[expression.Substring(indexOfOperator + 1)], variable, exchanges, fullDerivative) + ")";
                case 10:
                    indexOfOperator = expression.IndexOf('-');
                    return "(" + GetDerivative(exchanges[expression.Substring(0, indexOfOperator)], variable, exchanges, fullDerivative) + "-" + GetDerivative(exchanges[expression.Substring(indexOfOperator + 1)], variable, exchanges, fullDerivative) + ")";
                case 11:
                    indexOfOperator = expression.IndexOf('*');
                    operand1 = ReverseExchange(expression.Substring(0, indexOfOperator));
                    operand2 = ReverseExchange(expression.Substring(indexOfOperator + 1));
                    return "(" + GetDerivative(exchanges[expression.Substring(0, indexOfOperator)], variable, exchanges, fullDerivative) + "*(" + operand2 + ")+(" + operand1 + ")*" + GetDerivative(exchanges[expression.Substring(indexOfOperator + 1)], variable, exchanges, fullDerivative) + ")";
                case 12:
                    indexOfOperator = expression.IndexOf('/');
                    operand1 = ReverseExchange(expression.Substring(0, indexOfOperator));
                    operand2 = ReverseExchange(expression.Substring(indexOfOperator + 1));
                    return "((" + GetDerivative(exchanges[expression.Substring(0, indexOfOperator)], variable, exchanges, fullDerivative) + "*(" + operand2 + ")-(" + operand1 + ")*" + GetDerivative(exchanges[expression.Substring(indexOfOperator + 1)], variable, exchanges, fullDerivative) + ")/(" + operand2 + ")^2)";
                case 13:
                    indexOfOperator = expression.IndexOf('^');
                    operand1 = ReverseExchange(expression.Substring(0, indexOfOperator));
                    operand2 = ReverseExchange(expression.Substring(indexOfOperator + 1));
                    string derivative1 = GetDerivative(exchanges[expression.Substring(0, indexOfOperator)], variable, exchanges, fullDerivative);
                    string derivative2 = GetDerivative(exchanges[expression.Substring(indexOfOperator + 1)], variable, exchanges, fullDerivative);
                    return "((" + operand1 + ")^(" + operand2 + "-1)*((" + operand1 + ")*log(" + operand1 + ")*" + derivative2 + "+(" + operand2 + ")*" + derivative1 + "))";
                default: return null;
            }
        }

        private string ReverseExchange(string name)
        {
            string result = "(" + exchanges[name] + ")";
            int index = 0;
            while ((index = result.IndexOf('u')) != -1)
            {
                string newName = "u";
                index++;
                char look;
                while (index < result.Length && char.IsDigit(look = result[index]))
                {
                    newName += look;
                    index++;
                }
                result = result.Remove(index - newName.Length, newName.Length);
                result = result.Insert(index - newName.Length, ReverseExchange(newName));
            }
            return result;
        }

        private int GetTypeOfExpression()
        {
            if (expression[0] == '-')
            {
                if (expression[1] == 'u')
                {
                    return 1;
                }
                if (char.IsLetter(expression[1]))
                {
                    if (expression.Substring(1) == "pi" || expression.Substring(1) == "e" || expression.Substring(1) == "PI" || expression.Substring(1) == "E")
                    {
                        return 3;
                    }
                    else
                    {
                        if (expression.Contains('('))
                        {
                            return 4;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
                if (char.IsDigit(expression[1]))
                {
                    return 3;
                }
            }

            //process the case of two operands

            if (expression.Contains('+'))
            {
                return 9;
            }
            if (expression.Contains('-'))
            {
                return 10;
            }
            if (expression.Contains('*'))
            {
                return 11;
            }
            if (expression.Contains('/'))
            {
                return 12;
            }
            if (expression.Contains('^'))
            {
                return 13;
            }
            //to here
            if (expression[0] == 'u')
            {
                return 5;
            }
            if (char.IsLetter(expression[0]))
            {
                if (expression == "pi" || expression == "e" || expression == "PI" || expression == "E")
                {
                    return 7;
                }
                else
                {
                    if (expression.Contains('('))
                    {
                        return 8;
                    }
                    else
                    {
                        return 6;
                    }
                }
            }
            if (char.IsDigit(expression[0]))
            {
                return 7;
            }
            throw new Exception("unexcepted expression " + expression);
        }

        private string GetNameFromLastSymbolIndex(int i)
        {
            string result = "";
            while (i >= 0 && char.IsLetter(expression[i]))
            {
                result = result.Insert(0, "" + expression[i]);
                i--;
            }
            return result;
        }

        private void DeleteWhiteSpace()
        {
            for (int i = 0; i < expression.Length; i++)
            {
                if (Char.IsWhiteSpace(expression[i]))
                {
                    expression = expression.Remove(i, 1);
                }
            }
        }

        private bool Has(string subst, int start)
        {
            return expression.IndexOf(subst, start) != -1;
        }

        private bool IsChangedName(string s)
        {
            if (s[0] == 'u')
            {
                for (int i = 1; i < s.Length; i++)
                {
                    if (!char.IsDigit(s[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            else return false;
        }

        private static string GetDerivative(string expression, string variable, Dictionary<string, string> exchanges, bool fullDerivative)
        {
            return new Derivative(expression, variable, exchanges, fullDerivative).ComputeDerivative();
        }

        public static string GetDerivative(string expression, string variable, bool fullDerivative)
        {
            return new Derivative(expression, variable, null, fullDerivative).ComputeDerivative();
        }
    }
}
