namespace NumberSystemsCalculator.Services
{
    public static class InputValidator
    {
        public static void ValidateCalculationInput(string number1, string number2, string operation)
        {
            if (string.IsNullOrWhiteSpace(number1))
                throw new ArgumentException("Первое число не может быть пустым.");
            
            if (string.IsNullOrWhiteSpace(number2))
                throw new ArgumentException("Второе число не может быть пустым.");
            
            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("Операция не может быть пустой.");
        }

        public static void ValidateDivision(string number2, string operation)
        {
            if (operation == "/")
            {
                // Проверяем, что второе число не равно нулю
                if (string.IsNullOrWhiteSpace(number2) || number2.Trim() == "0")
                    throw new DivideByZeroException("Деление на ноль невозможно.");
            }
        }

        public static void ValidateBase(int baseValue, string parameterName)
        {
            if (baseValue < 2 || baseValue > 16)
                throw new ArgumentException($"Основание {parameterName} должно быть в диапазоне от 2 до 16.");
        }

        public static void ValidateOperation(string operation)
        {
            var supportedOperations = new[] { "+", "-", "*", "/" };
            if (!supportedOperations.Contains(operation))
                throw new ArgumentException($"Неподдерживаемая операция: {operation}");
        }

        public static bool IsValidNumberForBase(string number, int baseValue)
        {
            if (string.IsNullOrWhiteSpace(number))
                return false;

            foreach (char digit in number.Replace('.', '0').Replace(',', '0'))
            {
                if (!IsValidDigitForBase(digit, baseValue))
                    return false;
            }

            return true;
        }

        private static bool IsValidDigitForBase(char digit, int baseValue)
        {
            if (char.IsDigit(digit))
            {
                int value = digit - '0';
                return value < baseValue;
            }
            else if (char.IsLetter(digit))
            {
                int value = char.ToUpper(digit) - 'A' + 10;
                return value < baseValue;
            }

            return false;
        }
    }
} 