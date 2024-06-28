using Dake.DAL;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dake.Attributes.ValidationAttributes
{
    public class NoBannedWordsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var context = validationContext.GetService<Context>();

            var setting = context.Settings.FirstOrDefault();

            var wrongWordList = setting.wrongWord.Split('-');

            var input = value as string;

            var words = input.Split(' ');

            var response = words.All(word => !wrongWordList.Contains(word))
                ? ValidationResult.Success
                : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return response;
        }
    }
}
