using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Eticaret.ViewModels;
using Eticaret.Models;
using Microsoft.AspNetCore.Identity;

namespace Eticaret.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IdentityUserRoleY> UserRolesY { get; set; }
        public DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }



    }
}