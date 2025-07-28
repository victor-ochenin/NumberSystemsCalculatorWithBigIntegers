namespace NumberSystemsCalculator.Services
{
    public interface INumberSystemCalculator
    {
        string Calculate(string number1, int base1, string number2, int base2, string operation, int resultBase);
        Task<string> CalculateAsync(string number1, int base1, string number2, int base2, string operation, int resultBase);
    }
} 