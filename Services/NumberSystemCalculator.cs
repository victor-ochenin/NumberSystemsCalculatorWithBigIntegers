using System.Numerics;
using System.Threading.Tasks;
using System;

namespace NumberSystemsCalculator.Services
{
    public static class NumberSystemCalculator
    {
        public static string Calculate(string number1, int base1, string number2, int base2, string operation, int resultBase)
        {
            return CalculateAsync(number1, base1, number2, base2, operation, resultBase).GetAwaiter().GetResult();
        }

        public static async Task<string> CalculateAsync(string number1, int base1, string number2, int base2, string operation, int resultBase)
        {
            InputValidator.ValidateCalculationInput(number1, number2, operation);

            BigInteger bigIntValue1, bigIntValue2;
            decimal fractionalValue1 = 0, fractionalValue2 = 0;
            
            try
            {
                var task1 = Task.Run(() => NumberConverter.ConvertToBigInteger(number1, base1));
                var task2 = Task.Run(() => NumberConverter.ConvertToBigInteger(number2, base2));
                
                await Task.WhenAll(task1, task2);
                
                (bigIntValue1, fractionalValue1) = await task1;
                (bigIntValue2, fractionalValue2) = await task2;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Некорректное значение для указанной системы счисления: {ex.Message}");
            }

            var (bigIntResult, fractionalResult) = BigNumberOperations.PerformOperation(
                operation, bigIntValue1, fractionalValue1, bigIntValue2, fractionalValue2);

            return await Task.Run(() => NumberConverter.ConvertFromBigInteger(bigIntResult, fractionalResult, resultBase));
        }
    }
} 