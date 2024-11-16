using NUnit.Framework;

namespace JustAnother.Model.Tests;


[TestFixture]
public class DateInFutureAttributeTests
{

    [TestCase(10, ExpectedResult = true)]
    [TestCase(-10, ExpectedResult = false)]
    public bool DateValidation_InputDateRange_ReturnDateValidityResult(int addedTime)
    {
        DateInFutureAttribute dateInFutureAttribute = new DateInFutureAttribute();

        return dateInFutureAttribute.IsValid(DateTime.UtcNow.AddMinutes(addedTime));
    }

    [Test]
    public void DateValidation_InputAnyDate_ReturnErrorMessage()
    {
        DateInFutureAttribute dateInFutureAttribute = new DateInFutureAttribute();

        var result = "The date must be in future";

        Assert.That(result, Is.EqualTo(dateInFutureAttribute.ErrorMessage));

    }

}
