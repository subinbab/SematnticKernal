using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SemanticAIApp.Models;

namespace SemanticAIApp.Repository
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<SSOToken> SSOTokens { get; set; }
    }
}
