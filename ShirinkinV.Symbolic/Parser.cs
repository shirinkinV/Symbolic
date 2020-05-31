using ShirinkinV.Symbolic.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ShirinkinV.Symbolic
{
    public class Parser
    {

        #region Companion static functions
        public static int CharToInt(char c)
        {
            return Convert.ToInt32(c) - Convert.ToInt32('0');
        }
        private static bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }
        private static bool IsAlpha(char c)
        {
            return char.IsLetter(c) || c == '_';
        }
        private static bool IsDerSign(char c)
        {
            return c == '\'';
        }
        private static bool IsAddOperator(char c)
        {
            return "+-".Contains(c);
        }
        private static bool IsMulOperator(char c)
        {
            return "*/".Contains(c);
        }
        private static bool IsCaret(char c)
        {
            return c == '^';
        }
        private static bool IsWhiteSpace(char c)
        {
            return Char.IsWhiteSpace(c);
        }
        private static void Expected(string what)
        {
            throw new ParserException(what + " expected");
        }

        #endregion Companion static functions

        private string expression;
        private int position;
        private char look;
        public readonly Dictionary<string, Func<double, double>> builtInFunctions
            = new Dictionary<string, Func<double, double>>();
        private Dictionary<string, int> variables
            = new Dictionary<string, int>();

        #region Constructor
        private Parser(string expression, string[] variables)
        {
            InitBuiltinFunctions();
            this.expression = expression;
            if (variables != null)
                for (int i = 0; i < variables.Length; i++)
                {
                    this.variables[variables[i]] = i;
                }
            Reset();
        }
        #endregion Constructor

        #region Text operations
        public void InitBuiltinFunctions()
        {
            builtInFunctions["abs"] = Math.Abs;
            builtInFunctions["acos"] = Math.Acos;
            builtInFunctions["acosh"] = MathNet.Numerics.Trig.Acosh;
            builtInFunctions["asin"] = Math.Asin;
            builtInFunctions["asinh"] = MathNet.Numerics.Trig.Asinh;
            builtInFunctions["atan"] = Math.Atan;
            builtInFunctions["atanh"] = MathNet.Numerics.Trig.Atanh;
            builtInFunctions["cbrt"] = x => Math.Pow(x, 1 / 3);
            builtInFunctions["ceil"] = Math.Ceiling;
            builtInFunctions["cos"] = Math.Cos;
            builtInFunctions["cosh"] = Math.Cosh;
            builtInFunctions["exp"] = Math.Exp;
            builtInFunctions["floor"] = Math.Floor;
            builtInFunctions["log"] = Math.Log;
            builtInFunctions["log10"] = Math.Log10;
            builtInFunctions["signum"] = x => Math.Sign(x);
            builtInFunctions["sin"] = Math.Sin;
            builtInFunctions["sinh"] = Math.Sinh;
            builtInFunctions["sqrt"] = Math.Sqrt;
            builtInFunctions["tan"] = Math.Tan;
            builtInFunctions["tanh"] = Math.Tanh;
            builtInFunctions["rtd"] = x => x / Math.PI * 180;
            builtInFunctions["dtr"] = x => x * Math.PI / 180;
        }

        private void SkipWhiteSpace()
        {
            while (IsWhiteSpace(look))
            {
                GetChar();
            }
        }

        private double GetNum()
        {
            string result = "";
            if (!IsDigit(look)) Expected("Number in position " + position);
            while (IsDigit(look))
            {
                result += look;
                GetChar();
            }
            if (look == '.' || look == ',')
            {
                result += '.';
                GetChar();

                while (IsDigit(look))
                {
                    result += look;
                    GetChar();
                }
            }
            if (look == 'E' || look == 'e')
            {
                result += look;
                GetChar();
                result += look;
                GetChar();

                while (IsDigit(look))
                {
                    result += look;
                    GetChar();
                }
            }
            SkipWhiteSpace();
            return double.Parse(result, CultureInfo.InvariantCulture.NumberFormat);
        }

        private string GetName()
        {
            string result = "";
            if (!IsAlpha(look)) Expected("Name");
            while (IsDigit(look) || IsAlpha(look) || IsDerSign(look) || look == '{' || look == '}')
            {
                result += look;
                GetChar();
            }
            SkipWhiteSpace();
            return result;
        }

        private char Read()
        {
            if (position < expression.Length)
                return expression[position++];
            return '\u0000';
        }

        private void GetChar()
        {
            look = Read();
        }

        private void Match(char c)
        {
            if (look == c)
            {
                GetChar();
                SkipWhiteSpace();
            }
            else
                Expected(string.Format("'{0}'", c));
        }

        public void Reset()
        {
            position = 0;
            GetChar();
            SkipWhiteSpace();
        }
        #endregion Text operations

        public CommonF Parse()
        {
            Reset();
            return Expression();
        }

        #region Expression nodes
        //get expression as graph
        private CommonF Expression()
        {
            Sum sum = new Sum();

            Constant constant = new Constant(0);

            CommonF term = null;
            if (!IsAddOperator(look))
            {
                term = Term();
                if (!term.IsZero())
                {
                    if (!(term is Constant))
                    {
                        sum += term;
                    }
                    else
                    {
                        constant += (Constant)term;
                    }
                }
            }
            while (IsAddOperator(look))
            {

                switch (look)
                {
                    case '+':
                        Match('+');
                        term = Term();
                        if (!term.IsZero())
                        {
                            if (!(term is Constant))
                            {
                                sum += term;
                            }
                            else
                            {
                                constant += (Constant)term;
                            }
                        }
                        break;
                    case '-':
                        Match('-');
                        term = Term();
                        if (!term.IsZero())
                        {
                            if (!(term is Constant))
                            {
                                sum -= term;
                            }
                            else
                            {
                                constant -= (Constant)term;
                            }
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }

            if (sum.operands.Count == 0)
            {
                return constant;
            }
            if (constant.Value != 0)
            {
                sum += constant;
            }
            return sum;
        }

        private CommonF Term()
        {
            Mul mul = new Mul();
            Constant constant = new Constant(1);
            CommonF factor = Factor();
            if (!factor.IsZero())
            {
                if (!(factor is Constant))
                {
                    mul *= factor;
                }
                else
                {
                    constant *= (Constant)factor;
                }
            }
            else
            {
                while (IsMulOperator(look))
                {
                    switch (look)
                    {
                        case '*':
                            Match('*'); break;
                        case '/':
                            Match('/'); break;
                    }
                    Factor();
                }
                return new Constant(0);
            }
            while (IsMulOperator(look))
            {
                switch (look)
                {
                    case '*':
                        Match('*');
                        factor = Factor();
                        if (!factor.IsZero())
                        {
                            if (!(factor is Constant))
                                mul *= factor;
                            else
                                constant *= (Constant)factor;
                        }
                        else
                        {
                            while (IsMulOperator(look))
                            {
                                switch (look)
                                {
                                    case '*':
                                        Match('*'); break;
                                    case '/':
                                        Match('/'); break;
                                }
                                Factor();
                            }
                            return new Constant(0);
                        }
                        break;
                    case '/':
                        Match('/');
                        factor = Factor();
                        if (!factor.IsZero())
                        {
                            if (!(factor is Constant))
                                mul /= factor;
                            else
                                constant /= (Constant)factor;
                        }
                        else
                            throw new ArithmeticException("null division");
                        break;
                    default:
                        throw new Exception("unexpected parser error");
                }
            }

            if (constant.IsZero()) return new Constant(0);
            if (mul.operands.Count == 0) return constant;
            if (mul.operands.Count == 1)
            {
                if (constant.Value == 1)
                    return mul.operands[0];
                else
                    return mul.operands[0] * constant;
            }

            if (constant.Value == 1)

                return mul;
            else
                return mul * constant;
        }

        private CommonF Factor()
        {
            Pow pow = new Pow();
            CommonF power = Power();
            if (!power.IsZero())
            {
                pow.RaiseTo(power);
            }
            else
            {
                while (IsCaret(look))
                {
                    Match('^');
                    Power();
                }
                return new Constant(0);
            }
            while (IsCaret(look))
            {
                Match('^');
                power = Power();
                if (!power.IsZero())
                {
                    pow.RaiseTo(power);
                }
                else
                {
                    if (pow.baseAndPower.Count > 1)
                    {
                        pow.baseAndPower.RemoveAt(pow.baseAndPower.Count - 1);
                        while (IsCaret(look))
                        {
                            Match('^');
                            Power();
                        }
                        return pow;
                    }
                    else
                    {
                        while (IsCaret(look))
                        {
                            Match('^');
                            Power();
                        }
                        return new Constant(1);
                    }
                }
            }
            if (pow.baseAndPower.Count == 1)
            {
                return pow.baseAndPower[0];
            }
            else
            {
                return pow;
            }
        }

        private CommonF Power()
        {
            CommonF result = null;
            if (look == '(')
            {
                Match('(');
                result = Expression();
                Match(')');
                return result;
            }
            if (IsAlpha(look))
            {
                var name = GetName();
                if (look == '(')
                {
                    Match('(');
                    var arg = Expression();
                    result = new OneVar(builtInFunctions[name], arg, name + "(argument)");
                    Match(')');
                }
                else
                {
                    if (variables.ContainsKey(name))
                        result = new Variable(variables[name], name);
                    else
                    {
                        if (name == "PI" || name == "pi" || name == "Pi")
                        {
                            return new PI();
                        }
                        if (name == "e" || name == "E")
                        {
                            return new E();
                        }
                        variables[name] = variables.Count != 0 ? variables.Last().Value + 1 : 0;
                        result = new Variable(variables[name], name);
                    }
                    ((Variable)result).name = name;
                }
            }
            else
            {
                double constant = GetNum();
                result = new Constant(constant);
            }

            return result;
        }
        #endregion Expression nodes

        #region External public access functions

        public static Func<double[], double> ParseExpression(string expression, string[] variables)
        {
            return new Parser(expression, variables).Parse().Invoke;
        }

        public static CommonF ParseExpressionObject(string expression, string[] variables)
        {
            return new Parser(expression, variables).Parse();
        }

        public static string Optimize(string src)
        {
            string oldResult = ParseExpressionObject(src, null).Print();
            string newResult = ParseExpressionObject(oldResult, null).Print();
            newResult = DeleteBrackeds(newResult);
            while (newResult != oldResult)
            {
                oldResult = newResult;
                newResult = ParseExpressionObject(oldResult, null).Print();
                newResult = DeleteBrackeds(newResult);
            }
            return newResult;
        }

        public static string OptimizeToLatex(string src)
        {
            var oldResult = ParseExpressionObject(src, null).Print();
            var newResult = ParseExpressionObject(oldResult, null).PrintLatex();
            newResult = DeleteBrackedsLatex(newResult);
            return newResult;
        }

        static string DeleteBrackeds(string src)
        {
            int indexOfOpen = -1;
            while ((indexOfOpen = src.IndexOf('(', indexOfOpen + 1)) != -1)
            {
                int indexOfClose = -1;
                int opened = 1;
                for (int i = indexOfOpen + 1; i < src.Length; i++)
                {
                    if (src[i] == '(')
                        opened++;
                    if (src[i] == ')')
                        opened--;
                    if (opened == 0)
                    {
                        indexOfClose = i;
                        break;
                    }
                }
                while (src[indexOfOpen + 1] == '(' && src[indexOfClose - 1] == ')')
                {
                    src = src.Remove(indexOfClose - 1, 1).Remove(indexOfOpen + 1, 1);
                    indexOfClose -= 2;
                }
            }
            return src;
        }

        static string DeleteBrackedsLatex(string src)
        {
            int indexOfOpen = -1;
            while ((indexOfOpen = src.IndexOf('(', indexOfOpen + 1)) != -1)
            {
                int indexOfClose = -1;
                int opened = 1;
                for (int i = indexOfOpen + 1; i < src.Length; i++)
                {
                    if (src[i] == '(')
                        opened++;
                    if (src[i] == ')')
                        opened--;
                    if (opened == 0)
                    {
                        indexOfClose = i;
                        break;
                    }
                }
                while (src[indexOfOpen + 6] == '(' && src[indexOfClose - 7] == ')')
                {
                    src = src.Remove(indexOfClose - 13, 7).Remove(indexOfOpen + 1, 6);
                    indexOfClose -= 13;
                }
            }
            return src;
        }
        #endregion External public access functions
    }
}
