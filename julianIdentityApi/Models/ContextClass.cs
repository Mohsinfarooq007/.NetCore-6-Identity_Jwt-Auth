using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace julianIdentityApi.Models
{
    public class ContextClass:IdentityDbContext
    {
        public ContextClass(DbContextOptions options):base(options) 
        { 
        }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.seedRoles(builder);
        }


       public void seedRoles(ModelBuilder builder) 
        {

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() {Name="Manager",ConcurrencyStamp="1",NormalizedName="MANAGER" },
                new IdentityRole() { Name = "Engineer", ConcurrencyStamp = "1", NormalizedName = "ENGINEER" }
                );
              
        }
    }
}
