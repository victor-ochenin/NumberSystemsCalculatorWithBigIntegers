using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using NumberSystemsCalculator.Models;
using NumberSystemsCalculator.Services;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        BaseOptions = new List<SelectListItem>();
    }

    [BindProperty]
    [Required(ErrorMessage = "Первое число обязательно для заполнения")]
    public string Number1 { get; set; } = string.Empty;
    
    [BindProperty]
    public int Base1 { get; set; } = 10;
    
    [BindProperty]
    [Required(ErrorMessage = "Второе число обязательно для заполнения")]
    public string Number2 { get; set; } = string.Empty;
    
    [BindProperty]
    public int Base2 { get; set; } = 10;
    
    [BindProperty]
    public string Operation { get; set; } = "+";
    
    [BindProperty]
    public int ResultBase { get; set; } = 10;
    
    public string? Result { get; set; }
    public string? Error { get; set; }

    public List<SelectListItem> BaseOptions { get; set; }

    public void OnGet()
    {
        _logger.LogInformation("OnGet called. Base1: {Base1}, Base2: {Base2}, Operation: {Operation}, ResultBase: {ResultBase}", Base1, Base2, Operation, ResultBase);
        PopulateBaseOptions();
    }

    public async Task OnPostAsync()
    {
        _logger.LogInformation("OnPost called. Number1: {Number1}, Base1: {Base1}, Number2: {Number2}, Base2: {Base2}, Operation: {Operation}, ResultBase: {ResultBase}", 
            Number1, Base1, Number2, Base2, Operation, ResultBase);

        PopulateBaseOptions(); 

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid.");
            
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            Error = errors.Any() ? string.Join(", ", errors) : "Пожалуйста, проверьте введенные данные.";
            Result = null;
            return;
        }

        try
        {
            _logger.LogInformation("Attempting calculation: {Number1} (base {Base1}) {Operation} {Number2} (base {Base2}) = ? (base {ResultBase})", 
                Number1, Base1, Operation, Number2, Base2, ResultBase);
            Result = await NumberSystemCalculator.CalculateAsync(Number1, Base1, Number2, Base2, Operation, ResultBase);
            Error = null;
            _logger.LogInformation("Calculation successful. Result: {Result}", Result);
        }
        catch (ArgumentException ex)
        {
            Result = null;
            Error = ex.Message;
            _logger.LogError(ex, "Validation failed: {Message}", ex.Message);
        }
        catch (DivideByZeroException ex)
        {
            Result = null;
            Error = ex.Message;
            _logger.LogError(ex, "Division by zero: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            Result = null;
            Error = "Произошла ошибка при вычислении. Пожалуйста, проверьте введенные данные.";
            _logger.LogError(ex, "Calculation failed: {Message}", ex.Message);
        }
    }

    private void PopulateBaseOptions()
    {
        var bases = new List<int> { 2, 8, 10, 16 };
        BaseOptions = bases.Select(b => new SelectListItem { Text = $"{GetBaseName(b)} ({b})", Value = b.ToString() }).ToList();
    }

    private string GetBaseName(int b)
    {
        return b switch
        {
            2 => "Двоичная",
            8 => "Восьмеричная",
            10 => "Десятичная",
            16 => "Шестнадцатеричная",
            _ => b.ToString()
        };
    }
} 