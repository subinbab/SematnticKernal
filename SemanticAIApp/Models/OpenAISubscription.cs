using Microsoft.AspNetCore.Identity;

namespace SemanticAIApp.Models
{
    public class OpenAISubscription
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int RateLimit { get; set; }
        public double TokenLimit { get; set; }
        public string description { get; set; }
    }
}
