using FluentResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MovieReservation.API.Extensions;

public static class ResultExtensions
{
    public static IError GetFirstError(this ResultBase result)
    {
        return result.Errors.First();
    }

    public static ModelStateDictionary AddValidationErrors(this ModelStateDictionary modelState, IDictionary<string, object> errorMetadata)
    {
        foreach (var error in errorMetadata)
        {
            modelState.AddModelError(error.Key, error.Value.ToString()!);
        }
        return modelState;
    }
}