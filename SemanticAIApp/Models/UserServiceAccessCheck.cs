using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;

namespace SemanticAIApp.Models
{
    public class UserServiceAccessCheck
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int RateRemaining { get; set; }
        public double TokenLinit { get; set; }
    }
}
