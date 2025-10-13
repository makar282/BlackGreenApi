using System.ComponentModel.DataAnnotations;

namespace SaveNature.Contracts
{
    public record LoginUserRequest(
        [Required] string UserName,
        [Required] string Password);
}