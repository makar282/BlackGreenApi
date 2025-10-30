using System.ComponentModel.DataAnnotations;

namespace BlackGreenApi.Core.Contracts
{
    public record RegisterUserRequest(
            [Required] string UserName,
            [Required] string Password);
}

