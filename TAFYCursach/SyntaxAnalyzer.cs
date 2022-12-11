using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFYCursach
{
    internal class SyntaxAnalyzer
    {
        public Dictionary<string, string> _initializedVariables = new Dictionary<string, string>();
        public List<string> operationsAssignments = new List<string>();
        public List<string> expression = new List<string>();

        private Form1 _form;
        private List<string> _identificators;
        private List<string> _numbers;

        private List<string> operations = new List<string> { "NE", "EQ", "LT", "LE", "GT", "GE", "plus", "min", "or", "mult", "div", "and", "~" };
        public SyntaxAnalyzer(Form1 form, List<string> identificators, List<string> numbers)
        {
            _form = form;
            _identificators = identificators;
            _numbers = numbers;
        }

        public void CheckProgram(string programStr)
        {
            if (!BracketCheck(programStr))// проверяем парность скобок
            {
                _form.CatchError($"Нарушена парность скобок");
            }

            string[] programStructure = ReferenceStrings.Program.Split(' ');
            string[] programStrArr = programStr.Split(' ');
            int p = 0;
            for (int i = 0; i < programStructure.Length; i++)
            {
                if (programStructure[i] == programStrArr[p])
                {
                    p++;
                    continue;
                }
                if (programStructure[i] == "{description}")
                {
                    p = Description(programStrArr, p);
                    if (p == -1)
                    {
                        _form.CatchError($"Неверное описание переменных в программе");
                        break;
                    }
                }

                if (programStructure[i] == "{body}")
                {
                    Body(programStrArr, p);
                }

            }
        }


        /// <summary>
        /// Инициализация переменных.
        /// </summary>
        public int Description(string[] str, int p)
        {
            string[] descriptionStructure = ReferenceStrings.Description.Split(' ');
            List<string> tempInd = new List<string>();
            for (int i = 0; i < descriptionStructure.Length; i++)
            {
                if (descriptionStructure[i] == "dim" && _identificators.Contains(str[p + 1]))
                {
                    //tempInd.Add(str[i]);
                    p++;
                    continue;
                }
                else if (descriptionStructure[i] == "{identifier}" && _identificators.Contains(str[p]))
                {
                    tempInd.Add(str[p]);
                    p++;
                    continue;
                }
                else if (descriptionStructure[i] == "{,}")
                {
                    if (str[p] == "," && _identificators.Contains(str[p + 1]))
                    {
                        p++;
                        i -= 2;
                        continue;
                    }

                }
                else if (descriptionStructure[i] == "type" && (str[p] == "int" || str[p] == "float" || str[p] == "bool"))
                {
                    while (tempInd.Count > 0)
                    {
                        _initializedVariables.Add(tempInd[tempInd.Count - 1], str[p]);
                        tempInd.RemoveAt(tempInd.Count - 1);
                    }
                    p++;
                    continue;
                }
                else
                {
                    return -1;
                }

            }

            if (str[p].Contains("dim"))
            {
                p = Description(str, p);
            }
            return p;
        }


        /// <summary>
        /// Разбор тела
        /// </summary>
        public void Body(string[] str, int p)
        {
            string[] bodyStructure = ReferenceStrings.Body.Split(' ');
            int pn = p;
            while (str[p] != "end")
            {
                if (p == str.Length - 1 && str[p] != "end")
                {
                    _form.CatchError("Программа должна заканчиваться на 'end'");
                    break;
                }
                while (str[p] == ":" || str[p] == "(" || str[p] == ")" || str[p] == "*")  // если мы встретили : или скобки комментария
                {
                    if (p == str.Length - 1 && str[p] != "end")
                    {
                        _form.CatchError("Программа должна заканчиваться на 'end'");
                        break;
                    }
                    p++;
                    pn++;
                }

                if (str[p] == "end")
                {
                    break;
                }

                if (CheckOperator(str, ref pn))
                {
                    p = pn;
                }
                else
                {
                    _form.CatchError($"Неверный синтаксис оператора {str[pn]}");
                    pn++;
                    p = pn;
                }
            }
        }


        public bool CheckOperator(string[] str, ref int p)
        {
            return isAssignment(str, ref p) || isFor(str, ref p) || isIf(str, ref p) || isWhile(str, ref p) || isWrite(str, ref p) || isReadLn(str, ref p);
        }

        public bool isIf(string[] str, ref int p)
        {
            if (str[p] == "if")
            {
                p++;
                if (str[p] == "(")
                {
                    p++;
                    while (str[p] != ")")
                    {
                        if (isExpression(str, ref p, true))
                        {
                            
                            if (CheckOperator(str, ref p))
                            {
                                if (str[p] == "else")
                                {
                                    p++;
                                    return false;
                                }
                                else if (str[p] == ")")
                                {
                                    p++;
                                    return CheckOperator(str, ref p);
                                }
                                else
                                {
                                    return false;
                                }
                            }

                        }
                    }
                    if (str[p] == ")")
                    {
                        p++;
                        return CheckOperator(str, ref p);
                    }
                }
            }
            return false;
        }

        private bool isReadLn(string[] str, ref int p)
        {
            if (str[p] == "readln")
            {
                p++;
                if (_identificators.Contains(str[p]))
                {
                    p++;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private bool isWrite(string[] str, ref int p)
        {
            if (str[p] == "writeln")
            {
                p++;
                if (isExpression(str, ref p, true))
                {
                    p++;
                    return true;
                }
            }
            return false;
        }

        private bool isWhile(string[] str, ref int p)
        {

            if (str[p] == "while")
            {
                p++;
                if (str[p] == "(")
                {
                    p++;
                    while (str[p] != ")")
                    {
                        if (isExpression(str, ref p, true))
                        {
                            if (CheckOperator(str, ref p))
                            {
                                if (str[p] == ")")
                                {
                                    p++;
                                    return CheckOperator(str, ref p);
                                }
                                else
                                {
                                    return false;
                                }

                            }
                        }
                    }
                    if (str[p] == ")")
                    {
                        p++;
                        return CheckOperator(str, ref p);
                    }
                }
            }
            
            return false;
        }

        private bool isFor(string[] str, ref int p)
        {
            if (str[p] == "for")
            {
                p++;

                if (isAssignment(str, ref p))
                {
                    if (str[p] == "to")
                    {
                        p++;
                        while (str[p] != "next")
                        {
                            if (isExpression(str, ref p, true))
                            {
                                if (str[p] == "step")
                                {
                                    p++;
                                    continue;
                                }
                                else if (str[p] != "next")
                                {
                                    p++;
                                    return CheckOperator(str, ref p);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }

                    }
                }
            
            }
            return false;
        }

        /// <summary>
        /// проверка оператора присваивания
        /// </summary>
        /// <returns></returns>
        public bool isAssignment(string[] str, ref int p)
        {
            if (_identificators.Contains(str[p]))
            {
                int startIndex = p;
                p++;
                if (str[p] == ":=")
                {
                    p++;
                    if (isExpression(str, ref p))
                    {
                        operationsAssignments.Add(string.Join(" ", str, startIndex, p - startIndex));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Проверка выражения.
        /// </summary>
        public bool isExpression(string[] str, ref int p, bool addExpression = false)
        {
            if (_identificators.Contains(str[p]) || _numbers.Contains(str[p]) || str[p] == "true" || str[p] == "false")
            {
                int temp = 0;
                int startIndex = p;
                p++;
                bool operation = false;
                while (str[p] != ":" &&
                    str[p] != ")" &&
                    str[p] != ";" &&
                    str[p] != "to" &&
                    str[p] != "else" &&
                    str[p] != "next")
                {
                    if ((_identificators.Contains(str[p]) || _numbers.Contains(str[p])) && operation)
                    {
                        operation = false;
                        p++;
                    }
                    else if (operations.Contains(str[p]) && !operation)
                    {
                        operation = true;
                        p++;
                    }
                    else if ((_identificators.Contains(str[p]) || _numbers.Contains(str[p])) && !operation)
                    {
                        break;
                    }
                    else if (str[p] == "writeln" ||
                    str[p] == "if" ||
                    str[p] == "for" ||
                    str[p] == "readln")
                    {
                        p--;
                        temp = 1;
                        break;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (addExpression)
                {
                    expression.Add(string.Join(" ", str, startIndex, p - startIndex + temp));
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Проверка на парность скобок.
        /// </summary>
        // <param name="s"></param>
        /// <returns></returns>
        static bool BracketCheck(string s)
        {
            string t = "[{(]})";
            Stack<char> st = new Stack<char>();
            foreach (var x in s)
            {
                int f = t.IndexOf(x);

                if (f >= 0 && f <= 2)
                    st.Push(x); ;

                if (f > 2)
                {
                    if (st.Count == 0 || st.Pop() != t[f - 3])
                        return false;
                }
            }

            if (st.Count != 0)
                return false;

            return true;
        }
    }

    public static class ReferenceStrings
    {
        public static string Program = "{description} {body} end";
        public static string Description = "dim {identifier} {,} type";
        public static string Body = " {{operator} : end/{operator}";
    }

}
