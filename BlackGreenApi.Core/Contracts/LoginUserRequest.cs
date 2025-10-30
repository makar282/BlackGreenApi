using System.ComponentModel.DataAnnotations;

namespace BlackGreenApi.Core.Contracts
{
    public record LoginUserRequest(
        [Required] string UserName,
        [Required] string Password);
}