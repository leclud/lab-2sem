using System.Globalization;

namespace Rpn_Logic
{
    abstract class Token
    {
        public new abstract string ToString();
    }

    class Parenthesis : Token
    {
        public bool IsClosing { get; set; }
        public int Priority = 0;

        public Parenthesis(char symbol)
        {
            if (symbol != '(' && symbol != ')')
                throw new ArgumentException("This is not a valid parenthesis");

            IsClosing = symbol == ')';
        }

        public override string ToString()
        {
            return IsClosing ? ")" : "(";
        }
    }

    class Number : Token
    {
        public double Value { get; }

        public Number(string str)
        {
            Value = double.Parse(str, CultureInfo.InvariantCulture);
        }

        public Number(double num)
        {
            Value = num;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static Number operator +(Number a, Number b)
        {
            return new Number(a.Value + b.Value);
        }

        public static Number operator -(Number a, Number b)
        {
            return new Number(a.Value - b.Value);
        }

        public static Number operator *(Number a, Number b)
        {
            return new Number(a.Value * b.Value);
        }

        public static Number operator /(Number a, Number b)
        {
            return new Number(a.Value / b.Value);
        }
    }

    class Variable : Token
    {
        public char Name { get; }

        public Variable(char name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}