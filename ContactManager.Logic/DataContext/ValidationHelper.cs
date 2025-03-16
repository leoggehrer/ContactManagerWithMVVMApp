using System.ComponentModel.DataAnnotations;

namespace ContactManager.Logic.DataContext
{
    public static class ValidationHelper
    {
        public static bool ValidateEntity(object entity, out List<string> errors)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(entity, null, null);
            bool isValid = Validator.TryValidateObject(entity, context, validationResults, true);

            errors = [.. validationResults.Select(r => r.ErrorMessage)];
            return isValid;
        }
    }
}
