using System.ComponentModel.DataAnnotations;

namespace ECommerce532.Validations;

//[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class LetterOnlyAttribute : ValidationAttribute
{
    private readonly int _minValue; 
    private readonly int _maxValue; 

    public LetterOnlyAttribute(int minValue = 0, int maxValue = 8000)
    {
        _minValue = minValue;
        _maxValue = maxValue;
    }

    public override bool IsValid(object? value)
    {
        if (value is not string text)
            return true;

        return text.Length >= _minValue && text.Length <= _maxValue && text.All(c => char.IsLetter(c));
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field {name} must have letter only, and the length must be between {_minValue} - {_maxValue}";
    }
}
