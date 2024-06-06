namespace Rpn_Logic
{
    static class TokenCreator
    {
        private static List<Operation> _availableOperations;

        public static Token Create(string input)
        {
            if (char.IsDigit(input.First())) return new Number(input);
            return CreateOperation(input);
        }

        public static Token Create(char symbol, List<Char> varNames)
        {
            if (varNames.Contains(symbol)) return new Variable(symbol);
            if (symbol == '(' || symbol == ')') return new Parenthesis(symbol);

            return CreateOperation(symbol.ToString());
        }

        public static Operation CreateOperation(string name)
        {
            var operation = FindAvailableOperationByName(name);
            return operation;
        }

        private static Operation FindAvailableOperationByName(string name)
        {
            if (_availableOperations == null)
            {
                var parent = typeof(Operation);
                var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                var types = allAssemblies.SelectMany(x => x.GetTypes());
                var inheritingTypes = types.Where(t => parent.IsAssignableFrom(t) && !t.IsAbstract).ToList();

                _availableOperations =
                    inheritingTypes.Select(type => (Operation)Activator.CreateInstance(type)).ToList();
            }

            return _availableOperations.FirstOrDefault(op => op.Name.Equals(name));
        }

    }

    public class RpnCalculator
    {
        private readonly List<Token> _rpn;
        private readonly List<char> _variableNames = ['x'];

        public RpnCalculator(string expression)
        {
            _rpn = ToRpn(GetTokens(expression));
        }

        public double Calculate(float varValue)
        {
            return CalculateExpression(varValue).Value;
        }

        private Number CalculateExpression(float varValue)
        {
            var numbers = new Stack<Number>();
            foreach (var token in _rpn)
            {
                if (token is Number num)
                {
                    numbers.Push(num);
                }
                else if (token is Variable variable)
                {
                    numbers.Push(new Number(varValue));
                }
                else
                {
                    var op = token as Operation;

                    var args = new Number[op.ArgsCount];
                    for (var i = op.ArgsCount - 1; i >= 0; i--)
                        args[i] = numbers.Pop();

                    var result = op.Execute(args);
                    numbers.Push(result);
                }
            }

            return numbers.Pop();
        }

        private List<Token> GetTokens(string input)
        {
            List<Token> tokens = new List<Token>();
            string token = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsDigit(c) || c == '.' || c == ',')
                {
                    token += (c == ',' ? '.' : c);
                }
                else if (char.IsLetter(c))
                {
                    if (!string.IsNullOrEmpty(token) && (char.IsDigit(token.Last()) || token.Last() == '.'))
                    {
                        tokens.Add(new Number(token));
                        token = string.Empty;
                    }
                    token += c;
                }
                else
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        var operation = TokenCreator.CreateOperation(token);
                        if (operation != null && operation.IsFunction)
                        {
                            tokens.Add(operation);
                        }
                        else if (char.IsLetter(token.First()))
                        {
                            tokens.Add(new Variable(token.First()));
                        }
                        else
                        {
                            tokens.Add(new Number(token));
                        }
                        token = string.Empty;
                    }

                    if (c == '(' || c == ')')
                    {
                        tokens.Add(TokenCreator.Create(c, _variableNames));
                    }
                    else
                    {
                        tokens.Add(TokenCreator.Create(c.ToString()));
                    }
                }
            }

            if (!string.IsNullOrEmpty(token))
            {
                var operation = TokenCreator.CreateOperation(token);
                if (operation != null && operation.IsFunction)
                {
                    tokens.Add(operation);
                }
                else if (char.IsLetter(token.First()))
                {
                    tokens.Add(new Variable(token.First()));
                }
                else
                {
                    tokens.Add(new Number(token));
                }
            }

            return tokens;
        }

        private List<Token> ToRpn(List<Token> tokens)
        {
            var rpn = new List<Token>();
            var stack = new Stack<Token>();
            foreach (var token in tokens)
            {
                if (token is Number || token is Variable)
                {
                    rpn.Add(token);
                }
                else if (token is Operation op)
                {
                    if (stack.Count == 0
                        || stack.Peek() is Parenthesis
                        || (stack.Peek() is Operation stackOp
                            && stackOp.Priority < op.Priority))
                    {
                        stack.Push(token);
                        continue;
                    }

                    while (true)
                    {
                        if (!stack.Any() || ((Operation)stack.Peek()).Priority < op.Priority)
                        {
                            break;
                        }

                        rpn.Add(stack.Pop());
                    }

                    stack.Push(token);
                }
                else if (token is Parenthesis parenthesis)
                {
                    if (!parenthesis.IsClosing)
                    {
                        stack.Push(parenthesis);
                        continue;
                    }

                    while (stack.Pop() is Operation current)
                    {
                        rpn.Add(current);
                    };
                }
            }

            if (stack.Count != 0)
            {
                while (stack.Count > 0)
                {
                    rpn.Add(stack.Pop());
                }
            }

            return rpn;
        }

    }
}