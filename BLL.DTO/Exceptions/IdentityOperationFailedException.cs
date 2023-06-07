using Microsoft.AspNetCore.Identity;

namespace BLL.DTO.Exceptions;

public class IdentityOperationFailedException : Exception
{
    public readonly IEnumerable<string> Errors;

    public IdentityOperationFailedException(params string[] errors) :
        base("Identity operation failed to complete")
    {
        Errors = errors;
    }

    public IdentityOperationFailedException(IEnumerable<IdentityError> errors) : base(
        "Identity operation failed to complete")
    {
        Errors = errors.Select(e => e.Description);
    }
}