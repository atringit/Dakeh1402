using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Dake.Attributes.ValidationAttributes
{
    public class PriceValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || (long)value == 0)
                return ValidationResult.Success;

            var configuration = validationContext.GetService<IConfiguration>();

            var isDetermined = int.TryParse(configuration.GetSection("MinimumAmount").Value, out int minValue);

            if (!isDetermined)
            {
                minValue = 5000;
            }

            return (long)value < minValue
                ? new ValidationResult(FormatErrorMessage(minValue.ToString()))
                : ValidationResult.Success;
        }
    }
}
