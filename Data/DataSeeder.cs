using Microsoft.EntityFrameworkCore;
using SmartGym.Models;

namespace SmartGym.Data
{
    public static class DataSeeder
    {
        public static void seedRole(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    Id = 1,
                    RoleName = "Customer"
                },
                new Role()
                {
                    Id = 2,
                    RoleName = "Staff"
                },
                new Role()
                {
                    Id = 3,
                    RoleName = "Trainer"
                },
                new Role()
                {
                    Id = 4,
                    RoleName = "Admin"
                }
            );
        }
    }
}