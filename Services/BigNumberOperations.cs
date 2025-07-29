using System.Numerics;

namespace NumberSystemsCalculator.Services
{
    public static class BigNumberOperations
    {
        public static (BigInteger integerPart, decimal fractionalPart) Add(
            BigInteger int1, decimal frac1, 
            BigInteger int2, decimal frac2)
        {
            BigInteger resultInt = int1 + int2;
            decimal resultFrac = frac1 + frac2;

            if (resultFrac >= 1)
            {
                resultInt += BigInteger.One;
                resultFrac -= 1;
            }
            else if (resultFrac < 0)
            {
                resultInt -= BigInteger.One;
                resultFrac += 1;
            }

            return (resultInt, resultFrac);
        }

        public static (BigInteger integerPart, decimal fractionalPart) Subtract(
            BigInteger int1, decimal frac1, 
            BigInteger int2, decimal frac2)
        {
            BigInteger resultInt = int1 - int2;
            decimal resultFrac = frac1 - frac2;

            if (resultFrac >= 1)
            {
                resultInt += BigInteger.One;
                resultFrac -= 1;
            }
            else if (resultFrac < 0)
            {
                resultInt -= BigInteger.One;
                resultFrac += 1;
            }

            return (resultInt, resultFrac);
        }

        public static (BigInteger integerPart, decimal fractionalPart) Multiply(
            BigInteger int1, decimal frac1, 
            BigInteger int2, decimal frac2)
        {
            BigInteger resultInt = int1 * int2;
            decimal resultFrac = frac1 * frac2;

            if (resultFrac >= 1)
            {
                resultInt += BigInteger.One;
                resultFrac -= 1;
            }

            return (resultInt, resultFrac);
        }

        public static (BigInteger integerPart, decimal fractionalPart) Divide(
            BigInteger int1, decimal frac1, 
            BigInteger int2, decimal frac2)
        {
            if (int2 == BigInteger.Zero && Math.Abs(frac2) < 0.0000001m)
                throw new DivideByZeroException("Деление на ноль невозможно.");

            decimal fullResult = (decimal)int1 / (decimal)int2;

            BigInteger resultInt = (BigInteger)Math.Floor(fullResult);
            decimal resultFrac = fullResult - (decimal)resultInt;

            if (Math.Abs(frac1) > 0.0000001m || Math.Abs(frac2) > 0.0000001m)
            {
                decimal fractionalResult = frac1 / (decimal)int2;
                resultFrac += fractionalResult;
                
                if (resultFrac >= 1)
                {
                    resultInt += BigInteger.One;
                    resultFrac -= 1;
                }
                else if (resultFrac < 0)
                {
                    resultInt -= BigInteger.One;
                    resultFrac += 1;
                }
            }

            return (resultInt, resultFrac);
        }

        public static (BigInteger integerPart, decimal fractionalPart) PerformOperation(
            string operation,
            BigInteger int1, decimal frac1, 
            BigInteger int2, decimal frac2)
        {
            return operation switch
            {
                "+" => Add(int1, frac1, int2, frac2),
                "-" => Subtract(int1, frac1, int2, frac2),
                "*" => Multiply(int1, frac1, int2, frac2),
                "/" => Divide(int1, frac1, int2, frac2),
                _ => throw new ArgumentException($"Неподдерживаемая операция: {operation}")
            };
        }
    }
} 