using Microsoft.AspNetCore.Identity;
using RetroBoardBackend.Dtos.Responses;

namespace RetroBoardBackend.Dtos.Result
{
    public class UserResult
    {
        public UserResponse? UserResponse { get; set; }
        public List<UserResponse>? UserResponses { get; set; }
        public IEnumerable<IdentityError> IdentityErrors { get; set; } = new List<IdentityError>();
        public string? ErrorMessage { get; set; }
    }
}
