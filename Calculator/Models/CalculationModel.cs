using System;
using System.Collections.Generic;

namespace Calculator.Models
{
    public class CalculationModel
    {
        #region Приватные свойства

        private string result { get; set; }

        private string postfixExpr { get; set; }

        #endregion

        #region Конструктор

        public CalculationModel()
        {
            InputExpression = string.Empty;
            PostfixExpr = string.Empty;
        }

        #endregion

        #region Публичные свойства и методы

        public string InputExpression { get; set; }

        public string PostfixExpr { get { return postfixExpr; } private set { } }

        public string Result { get { return result; } private set { } }

        public void CalculateResult()
        {
            postfixExpr = ToPostfix(InputExpression);

            result = Calc(postfixExpr).ToString();
        }

        #endregion

        //	Список и приоритет операторов
        private Dictionary<char, int> operationPriority = new Dictionary<char, int>() {
            {'(', 0},
            {')', 0},
            {'+', 1},
            {'-', 1},
            {'*', 2},
            {'/', 2},
            {'^', 3},
        };

        /// <summary>
        /// Находит результат выражения
        /// </summary>
        /// <param name="postfixExpr">Выражение в постфиксной записи</param>
        /// <returns>Результат выражения</returns>
        private double Calc(string postfixExpr)
        {
            //	Стек для хранения чисел
            Stack<double> locals = new Stack<double>();
            //	Счётчик действий
            int counter = 0;

            //	Проходим по строке
            for (int i = 0; i < postfixExpr.Length; i++)
            {
                char c = postfixExpr[i];

                if (Char.IsDigit(c))
                {
                    //	Парсим
                    string number = GetStringNumber(postfixExpr, ref i);
                    //	Заносим в стек, преобразовав из String в Double-тип
                    locals.Push(Convert.ToDouble(number));
                }
                //	Если символ есть в списке операторов
                else if (operationPriority.ContainsKey(c))
                {
                    counter += 1;
                    //	Получаем значения из стека в обратном порядке
                    double second = locals.Count > 0 ? locals.Pop() : 0,
                    first = locals.Count > 0 ? locals.Pop() : 0;
                    //	Получаем результат операции и заносим в стек
                    locals.Push(Execute(c, first, second));
                }
            }

            //	По завершению цикла возвращаем результат из стека
            return locals.Pop();
        }

        /// <summary>
        /// Вычисляет значения, согласно оператору
        /// </summary>
        /// <param name="op">Оператор</param>
        /// <param name="first">Первый операнд (перед оператором)</param>
        /// <param name="second">Второй операнд (после оператора)</param>
        /// <returns>Результат вычисления</returns>
        private double Execute(char op, double first, double second)
        {
            double res = 0;
            switch (op)
            {
                case '+':
                    res = first + second;
                    break;
                case '-':
                    res = first - second;
                    break;
                case '*':
                    res = first * second;
                    break;
                case '/':
                    res = first / second;
                    break;
                case '^':
                    res = Math.Pow(first, second);
                    break;
            }

            return res;
        }

        /// <summary>
        /// Преобразовывает выражение в постфиксную запись
        /// </summary>
        /// <param name="infixExpr">Входная строка</param>
        /// <returns>Входная строка в постфиксной форме</returns>
        private string ToPostfix(string infixExpr)
        {
            //Строка для парсинга оператора Sqrt
            string sqrt = "";
            //	Выходная строка, содержащая постфиксную запись
            string postfixExpr = "";
            //	Инициализация стека, содержащий операторы в виде символов
            Stack<char> stack = new Stack<char>();
            //	Перебираем строку
            for (int i = 0; i < infixExpr.Length; i++)
            {
                char c = infixExpr[i];

                if (Char.IsDigit(c))
                {
                    //	Парсим его, передав строку и текущую позицию, и заносим в выходную строку
                    postfixExpr += GetStringNumber(infixExpr, ref i) + " ";
                }

                else if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    //	Заносим в выходную строку из стека всё вплоть до открывающей скобки
                    while (stack.Count > 0 && stack.Peek() != '(')
                        postfixExpr += stack.Pop();
                    //	Удаляем открывающуюся скобку из стека
                    stack.Pop();
                    if (sqrt == "Sqrt")
                    {
                        c = '^';
                        sqrt = "";
                        char op = c;
                        //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                        while (stack.Count > 0 && (operationPriority[stack.Peek()] >= operationPriority[op]))
                            postfixExpr += stack.Pop();
                        //	Заносим в стек оператор
                        stack.Push(op);

                        postfixExpr += 0.5.ToString();
                    }
                }
                //	Проверяем, содержится ли символ в списке операторов
                else if (operationPriority.ContainsKey(c))
                {
                    char op = c;
                    //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                    while (stack.Count > 0 && (operationPriority[stack.Peek()] >= operationPriority[op]))
                        postfixExpr += stack.Pop();
                    //	Заносим в стек оператор
                    stack.Push(op);
                }
                else
                {
                    if (c == 'S' || c == 'q' || c == 'r' || c == 't') sqrt += c;
                }
            }
            //	Заносим все оставшиеся операторы из стека в выходную строку
            foreach (char op in stack)
                postfixExpr += op;

            return postfixExpr;
        }

        /// <summary>
        /// Парсинг целочисленных значений
        /// </summary>
        /// <param name="expr">Строка для парсинга</param>
        /// <param name="pos">Позиция</param>
        /// <returns>Число в виде строки</returns>
        private string GetStringNumber(string expr, ref int pos)
        {
            string strNumber = "";

            for (; pos < expr.Length; pos++)
            {
                char num = expr[pos];

                if (Char.IsDigit(num)) strNumber += num;
                else if (num == ',') strNumber += num;
                else
                {
                    pos--;
                    break;
                }
            }

            return strNumber;
        }
    }
}
