using System;
using System.Collections.Generic;
using System.Numerics;
namespace Lab4_OOP
{
    public interface IMyNumber<T> where T : IMyNumber<T>
    {
        T Add(T b);
        T Subtract(T b);
        T Multiply(T b);
        T Divide(T b);
    }

    public class MyFrac : IMyNumber<MyFrac>, IComparable<MyFrac>
    {
        private BigInteger nom;
        private BigInteger denom;

        public MyFrac(BigInteger nom, BigInteger denom)
        {
            if (denom == 0)
            {
                throw new DivideByZeroException("Знаменник дробу не може дорівнювати нулю.");
            }
            if (denom < 0)
            {
                nom = -nom;
                denom = -denom;
            }

            // Скорочуємо дріб (НСД)
            BigInteger gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(nom), denom);
            this.nom = nom / gcd;
            this.denom = denom / gcd;
        }

        public MyFrac(int nom, int denom) : this((BigInteger)nom, (BigInteger)denom)
        {
        }
        //5 -> 5/1
        public MyFrac(BigInteger nom) : this(nom, 1)
        {
        }
        public double DoubleValue()
        {
            return (double)nom / (double)denom;
        }
        public MyFrac Add(MyFrac that)
        {
            return new MyFrac(this.nom * that.denom + that.nom * this.denom, this.denom * that.denom);
        }

        public MyFrac Subtract(MyFrac that)
        {
            return new MyFrac(this.nom * that.denom - that.nom * this.denom, this.denom * that.denom);
        }

        public MyFrac Multiply(MyFrac that)
        {
            return new MyFrac(this.nom * that.nom, this.denom * that.denom);
        }

        public MyFrac Divide(MyFrac that)
        {
            if (that.nom == 0)
            {
                throw new DivideByZeroException("Ділення на нульовий дріб неможливе.");
            }
            return new MyFrac(this.nom * that.denom, this.denom * that.nom);
        }

        public static MyFrac operator +(MyFrac a, MyFrac b) => a.Add(b);
        public static MyFrac operator -(MyFrac a, MyFrac b) => a.Subtract(b);
        public static MyFrac operator *(MyFrac a, MyFrac b) => a.Multiply(b);
        public static MyFrac operator /(MyFrac a, MyFrac b) => a.Divide(b);

        public int CompareTo(MyFrac that)
        {
            if (that == null) return 1;
            // Порівнюємо a/b та c/d через порівняння a*d та c*b (знаменники завжди > 0)
            BigInteger left = this.nom * that.denom;
            BigInteger right = that.nom * this.denom;
            return left.CompareTo(right);
        }

        public override string ToString()
        {
            if (denom == 1) return nom.ToString();
            return $"{nom}/{denom}";
        }
    }

    public class MyComplex : IMyNumber<MyComplex>
    {
        private double re;
        private double im;

        public MyComplex(double re, double im)
        {
            this.re = re;
            this.im = im;
        }
        public MyComplex(double re) : this(re, 0)
        {
        }

        public MyComplex Add(MyComplex that)
        {
            return new MyComplex(this.re + that.re, this.im + that.im);
        }

        public MyComplex Subtract(MyComplex that)
        {
            return new MyComplex(this.re - that.re, this.im - that.im);
        }

        public MyComplex Multiply(MyComplex that)
        {
            // (ac - bd) + (ad + bc)i
            double newRe = this.re * that.re - this.im * that.im;
            double newIm = this.re * that.im + this.im * that.re;
            return new MyComplex(newRe, newIm);
        }

        public MyComplex Divide(MyComplex that)
        {
            // ((ac + bd) / (c^2 + d^2)) + ((bc - ad) / (c^2 + d^2))i
            double denominator = that.re * that.re + that.im * that.im;
            if (denominator == 0) // Перевірка на ділення на 0 + 0i
            {
                throw new DivideByZeroException("Ділення на нульове комплексне число неможливе.");
            }

            double newRe = (this.re * that.re + this.im * that.im) / denominator;
            double newIm = (this.im * that.re - this.re * that.im) / denominator;
            return new MyComplex(newRe, newIm);
        }

        public static MyComplex operator +(MyComplex a, MyComplex b) => a.Add(b);
        public static MyComplex operator -(MyComplex a, MyComplex b) => a.Subtract(b);
        public static MyComplex operator *(MyComplex a, MyComplex b) => a.Multiply(b);
        public static MyComplex operator /(MyComplex a, MyComplex b) => a.Divide(b);

        public override string ToString()
        {
            if (im >= 0)
                return $"{re}+{im}i";
            else
                return $"{re}{im}i";
        }
    }

    class Program
    {
        static void testAPlusBSquare<T>(T a, T b) where T : IMyNumber<T>
        {
            Console.WriteLine("=== Starting testing (a+b)^2=a^2+2ab+b^2 with a=" + a + ", b=" + b + " ===");
            T aPlusB = a.Add(b);
            Console.WriteLine("a = " + a);
            Console.WriteLine("b = " + b);
            Console.WriteLine("(a + b) = " + aPlusB);
            Console.WriteLine("(a+b)^2 = " + aPlusB.Multiply(aPlusB));
            Console.WriteLine(" = = = ");
            T curr = a.Multiply(a);
            Console.WriteLine("a^2 = " + curr);
            T wholeRightPart = curr;
            curr = a.Multiply(b); // ab
            curr = curr.Add(curr); // ab + ab = 2ab
            Console.WriteLine("2*a*b = " + curr);
            wholeRightPart = wholeRightPart.Add(curr);
            curr = b.Multiply(b);
            Console.WriteLine("b^2 = " + curr);
            wholeRightPart = wholeRightPart.Add(curr);
            Console.WriteLine("a^2+2ab+b^2 = " + wholeRightPart);
            Console.WriteLine("=== Finishing testing (a+b)^2=a^2+2ab+b^2 with a=" + a + ", b=" + b + " ===\n");
        }

        // (a - b) == (a^2 - b^2) / (a + b)
        static void testSquaresDifference<T>(T a, T b) where T : IMyNumber<T>
        {
            Console.WriteLine("=== Starting testing (a-b) = (a^2-b^2)/(a+b) with a=" + a + ", b=" + b + " ===");
            T aMinusB = a.Subtract(b);
            Console.WriteLine("a = " + a);
            Console.WriteLine("b = " + b);
            Console.WriteLine("(a - b) = " + aMinusB);

            T a2 = a.Multiply(a);
            T b2 = b.Multiply(b);
            T numerator = a2.Subtract(b2); // a^2 - b^2
            T denominator = a.Add(b);      // a + b

            Console.WriteLine("a^2 - b^2 = " + numerator);
            Console.WriteLine("a + b = " + denominator);

            // Тут перевіряється ділення (і може виникнути DivideByZeroException, якщо a + b = 0)
            T rightPart = numerator.Divide(denominator);
            Console.WriteLine("(a^2 - b^2) / (a + b) = " + rightPart);
            Console.WriteLine("=== Finishing testing (a-b) = (a^2-b^2)/(a+b) with a=" + a + ", b=" + b + " ===\n");
        }

        static void Main(string[] args)
        {
            // Тест квадрата суми (з умови)
            testAPlusBSquare(new MyFrac(1, 3), new MyFrac(1, 6));
            testAPlusBSquare(new MyComplex(1, 3), new MyComplex(1, 6));

            // Тест різниці квадратів (перевірка віднімання та ділення)
            testSquaresDifference(new MyFrac(1, 2), new MyFrac(1, 3));
            testSquaresDifference(new MyComplex(5, 2), new MyComplex(3, 1));

            // Перевірка виключення ділення на нуль (коли a + b = 0)
            Console.WriteLine("=== Testing DivideByZeroException ===");
            try
            {
                // Для 1/3 та -1/3 сума (a + b) дорівнює 0, тому відбудеться ділення на 0
                testSquaresDifference(new MyFrac(1, 3), new MyFrac(-1, 3));
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("Успішно перехоплено виняток для MyFrac: " + ex.Message);
            }
            try
            {
                testSquaresDifference(new MyComplex(2, 4), new MyComplex(-2, -4));
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("Успішно перехоплено виняток для MyComplex: " + ex.Message);
            }
            Console.WriteLine();
            Console.WriteLine("=== Testing Sorting of MyFrac ===");
            List<MyFrac> fractions = new List<MyFrac>
            {
                new MyFrac(3, 4),
                new MyFrac(1, 2),
                new MyFrac(-5, 6),
                new MyFrac(2, 3),
                new MyFrac(1, 10)
            };

            Console.WriteLine("До сортування: " + string.Join(", ", fractions));
            fractions.Sort(); // Використовує наш метод CompareTo
            Console.WriteLine("Після сортування: " + string.Join(", ", fractions));
            Console.ReadKey();
        }
    }
}