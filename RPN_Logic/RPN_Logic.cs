namespace RPN_Logic
{
    public abstract class Token
    {

    }
    class Number : Token
    {
        public double Value { get; }
        public char Variable { get; }

        public Number(double value)
        {
            Value = value;
        }
        public Number(char variable)
        {
            Variable = variable;
        }

        public static bool CheckVariable(char variable)
        {
            return variable is 'x' or 'X' or 'х' or 'Х';
        }
    }
    class Operation : Token
    {
        public char Symbol;
        public int Priorety;
        public Operation(char symbol)
        {
            Symbol = symbol;
            Priorety = GetPriorety(symbol);
        }

        private int GetPriorety(char symbol)
        {
            Dictionary<object, int> PriorityOperation = new Dictionary<object, int>
            {
                {'+', 1},
                {'-', 1},
                {'*', 2},
                {'/', 2},
                {'(', 0},
                {')', 5},
            };
            return PriorityOperation[symbol];
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
    class Paranthesis(char symbol) : Token
    {
        public bool IsClosing { get; } = symbol == ')';
    }
    class GetToken
    {
        public static List<Token> Gettoken(string expression)
        {
            List<Token> tokens = new List<Token>();
            string number = string.Empty;
            foreach (var c in expression)
            {
                if (char.IsDigit(c))
                {
                    number += c;
                }
                else if (c == ',' || c == '.')
                {
                    number += ",";
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/')
                {
                    if (number != string.Empty)
                    {
                        tokens.Add(new Number(double.Parse(number)));
                        number = string.Empty;
                    }
                    tokens.Add(new Operation(c));
                }
                else if (c == '(' || c == ')')
                {
                    if (number != string.Empty)
                    {
                        tokens.Add(new Number(double.Parse(number)));
                        number = string.Empty;
                    }
                    tokens.Add(new Paranthesis(c));
                }
                else if (Number.CheckVariable(c))
                {
                    tokens.Add(new Number(c));
                }
            }

            if (number != string.Empty)
            {
                tokens.Add(new Number(double.Parse(number)));
            }

            return tokens;
        }

    }
    class ReversePolishNotation
    {
        public static List<Token> PRN(List<Token> tokens)
        {
            List<Token> rpn = new List<Token>();
            Stack<Token> operators = new Stack<Token>();

            foreach (Token token in tokens)
            {
                if (operators.Count == 0 && token is not Number)
                {
                    operators.Push(token);
                    continue;
                }

                if (token is Operation)
                {
                    if (operators.Peek() is Paranthesis)
                    {
                        operators.Push(token);
                        continue;
                    }

                    Operation first = (Operation)token;
                    Operation second = (Operation)operators.Peek();

                    if (first.Priorety > second.Priorety)
                    {
                        operators.Push(token);
                    }
                    else if (first.Priorety <= second.Priorety)
                    {
                        while (operators.Count > 0 && token is not Paranthesis)
                        {
                            rpn.Add(operators.Pop());
                        }
                        operators.Push(token);
                    }
                }
                else if (token is Paranthesis paranthesis)
                {
                    if (paranthesis.IsClosing)
                    {
                        while (operators.Peek() is not Paranthesis)
                        {
                            rpn.Add(operators.Pop());
                        }

                        operators.Pop();
                    }
                    else
                    {
                        operators.Push(paranthesis);
                    }
                }
                else if (token is Number num)
                {
                    rpn.Add(num);
                }
            }

            while (operators.Count > 0)
            {
                rpn.Add(operators.Pop());
            }
            return rpn;
        }

        public static double EvaluateRPN(List<Token> tokens)
        {
            Stack<double> stack = new Stack<double>();

            foreach (Token token in tokens)
            {
                if (token is Number)
                {
                    Number number = (Number)token;
                    stack.Push(number.Value);
                }
                else if (token is Operation)
                {
                    Operation op = (Operation)token;
                    if (stack.Count < 1)
                    {
                        throw new InvalidOperationException("Invalid expression");
                    }
                    double b = stack.Pop();
                    double a = stack.Pop();

                    switch (op.Symbol)
                    {
                        case '+':
                            stack.Push(a + b);
                            break;
                        case '-':
                            stack.Push(a - b);
                            break;
                        case '*':
                            stack.Push(a * b);
                            break;
                        case '/':
                            stack.Push(a / b);
                            break;
                    }
                }
            }

            if (stack.Count != 1)
            {
                throw new InvalidOperationException("Invalid expression");
            }

            return stack.Pop();
        }
    }
    public class RPNCalculator
    {
        public readonly double Result;
        public readonly List<Token> Rpn;
        public RPNCalculator(string expression)
        {
            Rpn = ReversePolishNotation.PRN(GetToken.Gettoken(expression));
            Result = CalculateWithoutX(Rpn);
        }
        public RPNCalculator(string expression, double variable)
        {
            List<Token> rpn = ReversePolishNotation.PRN(GetToken.Gettoken(expression));
            Result = CalculateWithX(rpn, variable);
        }
        private static double CalculateWithoutX(List<Token> rpnCalc)
        {
            Stack<double> tempCalc = new Stack<double>();

            foreach (Token token in rpnCalc)
            {
                if (token is Number num)
                {
                    tempCalc.Push(num.Value);
                }
                else if (token is Operation)
                {
                    double first = tempCalc.Pop();
                    double second = tempCalc.Pop();
                    var op = (Operation)token;
                    double result = CalculateOperation(first, second, op.Symbol);
                    tempCalc.Push(result);
                }
            }

            return tempCalc.Peek();
        }

        public double CalculateWithX(List<Token> rpnCalc, double variable)
        {
            Stack<double> tempCalc = new Stack<double>();

            foreach (Token token in rpnCalc)
            {
                if (token is Number num)
                {
                    tempCalc.Push(Number.CheckVariable(num.Variable) ? variable : num.Variable);
                }
                else if (token is Operation)
                {
                    double first = tempCalc.Pop();
                    double second = tempCalc.Pop();
                    var op = (Operation)token;
                    double result = CalculateOperation(first, second, op.Symbol);
                    tempCalc.Push(result);
                }
            }

            return tempCalc.Peek();
        }

        private static double CalculateOperation(double first, double second, char operation)
        {
            double result = 0;

            switch (operation)
            {
                case '+': result = first + second; break;
                case '-': result = second - first; break;
                case '*': result = first * second; break;
                case '/': result = second / first; break;
            }

            return result;
        }
    }
}
