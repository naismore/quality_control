using System;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 3)
            {
                Console.WriteLine("неизвестная ошибка");
                return 1;
            }

            int a, b, c;
            if (!int.TryParse(args[0], out a) ||
                !int.TryParse(args[1], out b) ||
                !int.TryParse(args[2], out c))
            {
                Console.WriteLine("неизвестная ошибка");
                return 1;
            }

            if (a <= 0 || b <= 0 || c <= 0)
            {
                Console.WriteLine("Не треугольник");
                return 0;
            }

            // Проверка условия существования треугольника
            if (a + b <= c || a + c <= b || b + c <= a)
            {
                Console.WriteLine("Не треугольник");
                return 0;
            }

            if (a == b && b == c)
            {
                Console.WriteLine("Равносторонний");
            }
            else if (a == b || a == c || b == c)
            {
                Console.WriteLine("Равнобедренный");
            }
            else
            {
                Console.WriteLine("Обычный");
            }

            return 0;
        }
        catch
        {
            Console.WriteLine("Неизвестная ошибка");
            return 1;
        }
    }
}