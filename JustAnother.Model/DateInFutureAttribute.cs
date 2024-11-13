using System.ComponentModel.DataAnnotations;

namespace JustAnother.Model;

public class DateInFutureAttribute : ValidationAttribute
{
    private Func<DateTime> _dateInFuture;

    public DateInFutureAttribute() : this(() => DateTime.UtcNow)
    {
        
    }

    public DateInFutureAttribute(Func<DateTime> dateInFuture)
    {
        _dateInFuture = dateInFuture;
        ErrorMessage = "The date must be in future";
    }


    public override bool IsValid(object? value)
    {
        bool isValid = false;

        if (value is DateTime dateTime)
        {
            isValid = dateTime > _dateInFuture();
        }

        return isValid;
    }



}
