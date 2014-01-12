using AssessmentNet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AssessmentNet.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));
                
            CreateRoleIfNotExists(roleManager, "admin");
            CreateRoleIfNotExists(roleManager, "testee");
            if(!context.Users.Any()) //first run user
                CreateUser(userManager, "admin", "admin");
        }

        private void CreateUser(UserManager<IdentityUser> userManager, string name, string role)
        {
            var x = userManager.Create(new ApplicationUser(name), "changeme");
            var u = userManager.FindByName(name);
            userManager.AddToRole(u.Id, role);
        }

        private static void CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string roleString)
        {
            if (!roleManager.RoleExists(roleString))
            {
                var role = new IdentityRole();
                role.Name = roleString;
                roleManager.Create(role);
            }
        }
    }
}
