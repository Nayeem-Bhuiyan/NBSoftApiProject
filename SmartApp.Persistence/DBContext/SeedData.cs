using Microsoft.EntityFrameworkCore;
using SmartApp.Domain.Entities.MasterData;
using System;

namespace SmartApp.Persistence.DBContext
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Seed_Countries();
            // Add more as needed
        }

        public static void Seed_Countries(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData(new Country
            {
                Id = 1,
                Name = "Bangladesh",
                Continent = "Asia",
                Code = "",
                PhoneCode = "+880",
                Currency = "Taka",
                IsActive = true,
                createdBy = "System",
                createdDate = new DateTime(2024, 01, 01),
                isDeleted = false
            });
        }

        // Add more as needed

    }
}
