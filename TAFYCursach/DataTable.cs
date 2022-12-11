using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFYCursach
{
    internal class DataTable
    {

        public static List<string> GetServiceWords()
        {
            return new List<string>()
            {
                "begin",
                "bool",
                "dim",
                "else",
                "end",
                "false",
                "float",
                "for",
                "if",
                "int",
                "next",
                "readln",
                "step",
                "to",
                "true",
                "while",
                "writeln"
            };
        }

        public static List<string> GetSeparators()
        {
            return new List<string>()
            {
                "NE",
                "EQ",
                "LT",
                "LE",
                "GT",
                "GE",
                "plus",
                "min",
                "or",
                "mult",
                "div",
                "and",
                "~",
                "(",
                ")",
                ".",
                ":",
                ",",
                ";",
                "*",
                ":="
            };
        }

    }
}
