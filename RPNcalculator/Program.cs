using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using RPN_Logic;



namespace LabWork
{
    class Programm
    {
        static void Main(string[] args)
        {
            Console.Write("Введите математическое выражение: ");
            string expression = Console.ReadLine();
            expression = expression.Replace(" ", string.Empty);

            Console.WriteLine("Результат: " + RPNCalculator.Calculator(expression));
            Console.ReadKey();
        }
    }
}