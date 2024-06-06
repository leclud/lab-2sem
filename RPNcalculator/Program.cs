using Rpn_Logic;



namespace LabWork
{
    class Programm
    {
        static void Main(string[] args)
        {
            Console.Write("Введите математическое выражение: ");
            string expression = Console.ReadLine();
            expression = expression.Replace(" ", string.Empty);
            RpnCalculator calculator = new RpnCalculator(expression);
            //
            Console.WriteLine("Результат: " );
            Console.ReadKey();
        }
    }
}