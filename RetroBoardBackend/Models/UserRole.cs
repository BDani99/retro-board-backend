using Microsoft.AspNetCore.Identity;

namespace RetroBoardBackend.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual Role Role { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
