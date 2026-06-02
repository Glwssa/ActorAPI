using FluentValidation.Results;

namespace ActorApi.Api.Extensions;

public static class ValidationResultExtensions
{
    public static IDictionary<string, string[]> ToValidationProblemDictionary(
        this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());
    }
}