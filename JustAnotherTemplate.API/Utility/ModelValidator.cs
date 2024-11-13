using System.ComponentModel.DataAnnotations;

namespace JustAnother.API.Utility;

public class ModelValidator
{
    public static bool TryValidate(object model, out List<ValidationResult> results)
    {
        var context = new ValidationContext(model, serviceProvider: null, items: null);

        results = new List<ValidationResult>();

        return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
    }
}
