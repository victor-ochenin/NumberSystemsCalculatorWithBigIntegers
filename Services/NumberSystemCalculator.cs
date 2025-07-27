namespace NumberSystemsCalculator.Services
{
    public static class NumberSystemCalculator
    {
        public static string Calculate(string number1, int base1, string number2, int base2, string operation, int resultBase)
        {
            if (!IsSupportedBase(base1) || !IsSupportedBase(base2) || !IsSupportedBase(resultBase))
                throw new ArgumentException("Поддерживаются только основания 2, 8, 10, 16.");

            decimal decimalValue1, decimalValue2;
            try
            {
                decimalValue1 = ConvertToDecimal(number1, base1);
                decimalValue2 = ConvertToDecimal(number2, base2);
            }
            catch
            {
                throw new ArgumentException($"Некорректное значение для указанной системы счисления.");
            }

            decimal result = operation switch
            {
                "+" => decimalValue1 + decimalValue2,
                "-" => decimalValue1 - decimalValue2,
                "*" => decimalValue1 * decimalValue2,
                "/" => decimalValue2 != 0 ? decimalValue1 / decimalValue2 : throw new DivideByZeroException("Деление на ноль невозможно."),
                _ => throw new ArgumentException($"Неподдерживаемая операция: {operation}")
            };

            if (resultBase == 10)
            {
                if (decimal.Truncate(result) == result)
                    return ((long)result).ToString();
                else
                    return result.ToString("F2").TrimEnd('0').TrimEnd('.');
            }

            int intResult = (int)Math.Floor(result);
            string convertedResult = ConvertFromInt(intResult, resultBase);
            
            decimal fractionalPart = result - intResult;
            if (fractionalPart > 0)
            {
                string fractionalString = ConvertFractionalPartToString(fractionalPart, resultBase);
                convertedResult += "." + fractionalString;
            }

            return convertedResult;
        }

        private static bool IsSupportedBase(int numberBase)
        {
            return numberBase == 2 || numberBase == 8 || numberBase == 10 || numberBase == 16;
        }

        private static decimal ConvertToDecimal(string value, int fromBase)
        {
            if (fromBase == 10)
            {
                string normalizedValue = value.Replace(',', '.');
                
                if (decimal.TryParse(normalizedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                    return result;
                    
                throw new ArgumentException($"Некорректное десятичное число: {value}");
            }

            string normalizedInput = value.Replace(',', '.');
            string[] parts = normalizedInput.Split('.');
            
            if (parts.Length > 2)
                throw new ArgumentException($"Некорректный формат числа: {value}");

            try
            {
                int intPart = System.Convert.ToInt32(parts[0], fromBase);
                decimal result = intPart;

                if (parts.Length == 2 && !string.IsNullOrEmpty(parts[1]))
                {
                    decimal fractionalPart = ConvertFractionalPart(parts[1], fromBase);
                    result += fractionalPart;
                }

                return result;
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Некорректное число '{value}' для системы счисления с основанием {fromBase}.");
            }
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

        private static int GetDigitValue(char digit, int fromBase)
        {
            if (char.IsDigit(digit))
            {
                int value = digit - '0';
                if (value >= fromBase)
                    throw new ArgumentException($"Цифра {digit} недопустима в системе с основанием {fromBase}");
                return value;
            }
            else if (char.IsLetter(digit))
            {
                int value = char.ToUpper(digit) - 'A' + 10;
                if (value >= fromBase)
                    throw new ArgumentException($"Цифра {digit} недопустима в системе с основанием {fromBase}");
                return value;
            }
            else
            {
                throw new ArgumentException($"Недопустимый символ: {digit}");
            }
        }

        private static string ConvertFromInt(int value, int toBase)
        {
            return System.Convert.ToString(value, toBase).ToUpper();
        }

        private static string ConvertFractionalPartToString(decimal fractionalPart, int fromBase)
        {
            string result = "";
            for (int i = 0; i < 10; i++)
            {
                fractionalPart *= fromBase;
                int digit = (int)Math.Floor(fractionalPart);
                result += GetDigitChar(digit, fromBase);
                fractionalPart -= digit;
            }
            return result;
        }

        private static char GetDigitChar(int digitValue, int fromBase)
        {
            if (digitValue < 10)
                return (char)('0' + digitValue);
            else
                return (char)('A' + (digitValue - 10));
        }
    }
} 