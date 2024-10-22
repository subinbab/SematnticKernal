using Microsoft.AspNetCore.Identity;
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
        public DbSet<OpenAISubscription> OpenAISubscription { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.SeedRoles(builder);
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() {  Name = "ADMIN", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole() {  Name = "ROLE1", ConcurrencyStamp = "2", NormalizedName = "ROLE1" },
                new IdentityRole() { Name = "ROLE2", ConcurrencyStamp = "3", NormalizedName = "ROLE2" }
                );
        }
    }
}
