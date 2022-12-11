using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFYCursach
{
    internal class ProgramTextHandler
    {

        //постановка пробелов перед разделителями
        public static string StartParser(string programStr)
        {
            while (programStr.Contains("\n")) { programStr = programStr.Replace("\n", " "); }
            while (programStr.Contains("\r")) { programStr = programStr.Replace("\r", " "); }
            for (int i = 0; i < programStr.Length; i++)
            {
                if (programStr[i] == ':')
                {
                    programStr = programStr.Insert(i, " ");
                    i++;
                }
                else if (programStr[i] == ',' || programStr[i] == '*' || programStr[i] == '(' || programStr[i] == ')' || programStr[i] == ';')
                {
                    programStr = programStr.Insert(i, " ");
                    programStr = programStr.Insert(i + 2, " ");
                    i += 2;
                }
            }
            while (programStr.Contains("  ")) { programStr = programStr.Replace("  ", " "); }


            //проверка на комментарии
            string tempStr = "";
            bool oneSymbolComment = false;
            bool twoSymbolComment = false;
            for (int i = 0; i < programStr.Count(); i++)
            {
                if (programStr[i] == '(' && programStr[i + 2] == '*')
                {
                    tempStr += programStr[i];
                    tempStr += programStr[i + 1];
                    tempStr += programStr[i + 2];
                    tempStr += " ";
                    oneSymbolComment = true;
                    twoSymbolComment = true;
                }

                if (programStr[i] == '*' && programStr[i + 2] == ')' && twoSymbolComment)
                {
                    twoSymbolComment = false;
                    oneSymbolComment = false;
                }

                if (!oneSymbolComment || !twoSymbolComment)
                {
                    tempStr += programStr[i];
                }
            }

            return tempStr;
        }

    }
}
