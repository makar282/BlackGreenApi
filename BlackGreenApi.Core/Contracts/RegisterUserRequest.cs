using System.ComponentModel.DataAnnotations;

namespace SaveNature.Contracts
{
    public record RegisterUserRequest(
            [Required] string UserName,
            [Required] string Password);
}

