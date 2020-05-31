using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic
{
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        { }
    }
}
