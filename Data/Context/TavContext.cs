using Entities.Models;
using Entities.Models.Roles;
using Entities.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Principal;


public class TavContext : DbContext
{
    public TavContext(DbContextOptions<TavContext> options)
        : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }
    //public DbSet<NewsReceiver> NewsReceivers { get; set; }
    public DbSet<EmailRecord> EmailRecords { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Administrator Role", IsDelete = false },
            new Role { Id = 2, Name = "User", Description = "User Role", IsDelete = false }
        );

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                UserName = "admin",
                Email = "alitaami81@gmail.com",
                PasswordHash = "8jr0nptvznD5VS2WniCx5y6jYyQOSw1ZpfsulA8c/3A=",
                FullName = "Admin",
                Age = 30,
                IsActive = true  
            },
            new User
            {
                Id = 2,
                UserName = "ali",
                Email = "alitaamicr7@gmail.com",
                PasswordHash = "8jr0nptvznD5VS2WniCx5y6jYyQOSw1ZpfsulA8c/3A=",
                FullName = "alitaami",
                Age = 21,
                IsActive = true 
            }
        );

        // Seed UserRoles
        modelBuilder.Entity<UserRoles>().HasData(
            new UserRoles { Id = 1, UserId = 1, RoleId = 1 }, // Assign Admin Role to User 1
            new UserRoles { Id = 2, UserId = 2, RoleId = 2 }  // Assign User Role to User 2
        );

        // Seed NewsReceivers
        //modelBuilder.Entity<NewsReceiver>().HasData(
        //    new NewsReceiver { Id = 1, Email = "alitaami81@gmail.com", UserId = 1 },
        //    new NewsReceiver { Id = 2, Email = "alitaamicr7@gmail.com", UserId = 2 }
        //);
    }

}



