using System;
using System.Configuration;

namespace TestingCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var st = ConfigurationManager.AppSettings["test"]; 
            Console.WriteLine(st);
        }
    }
}
