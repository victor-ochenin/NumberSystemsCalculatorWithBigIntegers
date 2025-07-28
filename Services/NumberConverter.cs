using System.Numerics;

namespace NumberSystemsCalculator.Services
{
    public static class NumberConverter
    {
        private static readonly Dictionary<string, (BigInteger, decimal)> _conversionCache = new();
        private static readonly object _cacheLock = new();

        public static (BigInteger integerPart, decimal fractionalPart) ConvertToBigInteger(string value, int fromBase)
        {
            string cacheKey = $"{value}_{fromBase}";
            lock (_cacheLock)
            {
                if (_conversionCache.TryGetValue(cacheKey, out var cachedResult))
                    return cachedResult;
            }

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Число не может быть пустым для основания {fromBase}.");

            (BigInteger intPart, decimal fracPart) result;

            if (fromBase == 10)
            {
                string normalizedValue = value.Replace(',', '.');
                
                if (decimal.TryParse(normalizedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal decimalResult))
                {
                    BigInteger intPart = (BigInteger)Math.Floor(decimalResult);
                    decimal fracPart = decimalResult - (decimal)intPart;
                    result = (intPart, fracPart);
                }
                else if (BigInteger.TryParse(normalizedValue, out BigInteger bigIntResult))
                {
                    result = (bigIntResult, 0);
                }
                else
                {
                    throw new ArgumentException($"Некорректное десятичное число '{value}' для основания {fromBase}.");
                }
            }
            else
            {
                string normalizedInput = value.Replace(',', '.');
                string[] parts = normalizedInput.Split('.');
                
                if (parts.Length > 2)
                    throw new ArgumentException($"Некорректный формат числа '{value}' для основания {fromBase}.");

                try
                {
                    BigInteger intPart = ConvertFromBase(parts[0], fromBase);
                    decimal fracPart = 0;

                    if (parts.Length == 2 && !string.IsNullOrEmpty(parts[1]))
                    {
                        fracPart = ConvertFractionalPart(parts[1], fromBase);
                    }

                    result = (intPart, fracPart);
                }
                catch (FormatException)
                {
                    throw new ArgumentException($"Некорректное число '{value}' для системы счисления с основанием {fromBase}.");
                }
            }

            lock (_cacheLock)
            {
                if (_conversionCache.Count > 1000)
                {
                    _conversionCache.Clear();
                }
                _conversionCache[cacheKey] = result;
            }

            return result;
        }

        public static string ConvertFromBigInteger(BigInteger integerPart, decimal fractionalPart, int toBase)
        {
            if (toBase == 10)
            {
                if (fractionalPart == 0)
                    return integerPart.ToString();
                else
                    return ((decimal)integerPart + fractionalPart).ToString();
            }

            string convertedResult = ConvertBigIntegerToString(integerPart, toBase);
            
            if (fractionalPart > 0)
            {
                string fractionalString = ConvertFractionalPartToString(fractionalPart, toBase);
                convertedResult += "." + fractionalString;
            }

            return convertedResult;
        }

        private static decimal ConvertFractionalPart(string fractionalPart, int fromBase)
        {
            decimal result = 0;
            decimal divisor = fromBase;

            foreach (char digit in fractionalPart)
            {
                int digitValue = GetDigitValue(digit, fromBase);
                result += digitValue / divisor;
                divisor *= fromBase;
            }

            return result;
        }

        private static string ConvertBigIntegerToString(BigInteger value, int toBase)
        {
            if (value == BigInteger.Zero) return "0";
            
            Span<char> buffer = stackalloc char[64];
            int index = 0;
            BigInteger temp = value;
            
            while (temp > BigInteger.Zero)
            {
                BigInteger remainder = temp % toBase;
                buffer[index++] = GetDigitChar((int)remainder, toBase);
                temp /= toBase;
            }
            
            var result = new char[index];
            for (int i = 0; i < index; i++)
            {
                result[i] = buffer[index - 1 - i];
            }
            
            return new string(result);
        }

        private static string ConvertFractionalPartToString(decimal fractionalPart, int fromBase)
        {
            Span<char> buffer = stackalloc char[20];
            int index = 0;
            decimal temp = fractionalPart;
            
            for (int i = 0; i < 20 && temp > 0; i++)
            {
                temp *= fromBase;
                int digit = (int)Math.Floor(temp);
                buffer[index++] = GetDigitChar(digit, fromBase);
                temp -= digit;
            }
            
            return new string(buffer.Slice(0, index)).TrimEnd('0');
        }

        private static int GetDigitValue(char digit, int fromBase)
        {
            if (char.IsDigit(digit))
            {
                int value = digit - '0';
                if (value >= fromBase)
                    throw new ArgumentException($"Цифра '{digit}' недопустима в системе с основанием {fromBase}");
                return value;
            }
            else if (char.IsLetter(digit))
            {
                int value = char.ToUpper(digit) - 'A' + 10;
                if (value >= fromBase)
                    throw new ArgumentException($"Цифра '{digit}' недопустима в системе с основанием {fromBase}");
                return value;
            }
            else
            {
                throw new ArgumentException($"Недопустимый символ '{digit}' для основания {fromBase}");
            }
        }

        private static char GetDigitChar(int digitValue, int fromBase)
        {
            if (digitValue < 10)
                return (char)('0' + digitValue);
            else
                return (char)('A' + (digitValue - 10));
        }

        private static BigInteger ConvertFromBase(string value, int fromBase)
        {
            Console.WriteLine($"[DEBUG] ConvertFromBase: value='{value}', fromBase={fromBase}");
            if (fromBase == 10)
            {
                return BigInteger.Parse(value);
            }
            // Убираем ветку для fromBase == 16, используем универсальный разбор
            BigInteger result = BigInteger.Zero;
            BigInteger power = BigInteger.One;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                int digitValue = GetDigitValue(value[i], fromBase);
                result += digitValue * power;
                power *= fromBase;
            }
            return result;
        }
    }
} 