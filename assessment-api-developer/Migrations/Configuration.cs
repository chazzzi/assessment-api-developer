namespace assessment_platform_developer.Migrations
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using assessment_platform_developer.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<AssessmentDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AssessmentDbContext context)
        {
            var countries = new List<Country>
            {
                new Country { ID = 1, Name = "Canada" },
                new Country { ID = 2, Name = "United States" }
            };

            context.Countries.AddOrUpdate(c => c.ID, countries.ToArray());

            // Seed States for United States
            var usStates = new List<State>
            {
                new State { ID = 1, Name = "Alabama", CountryID = 2 },
                new State { ID = 2, Name = "Alaska", CountryID = 2 },
                new State { ID = 3, Name = "Arizona", CountryID = 2 },
                new State { ID = 4, Name = "Arkansas", CountryID = 2 },
                new State { ID = 5, Name = "California", CountryID = 2 },
                new State { ID = 6, Name = "Colorado", CountryID = 2 },
                new State { ID = 7, Name = "Connecticut", CountryID = 2 },
                new State { ID = 8, Name = "Delaware", CountryID = 2 },
                new State { ID = 9, Name = "Florida", CountryID = 2 },
                new State { ID = 10, Name = "Georgia", CountryID = 2 },
                new State { ID = 11, Name = "Hawaii", CountryID = 2 },
                new State { ID = 12, Name = "Idaho", CountryID = 2 },
                new State { ID = 13, Name = "Illinois", CountryID = 2 },
                new State { ID = 14, Name = "Indiana", CountryID = 2 },
                new State { ID = 15, Name = "Iowa", CountryID = 2 },
                new State { ID = 16, Name = "Kansas", CountryID = 2 },
                new State { ID = 17, Name = "Kentucky", CountryID = 2 },
                new State { ID = 18, Name = "Louisiana", CountryID = 2 },
                new State { ID = 19, Name = "Maine", CountryID = 2 },
                new State { ID = 20, Name = "Maryland", CountryID = 2 },
                new State { ID = 21, Name = "Massachusetts", CountryID = 2 },
                new State { ID = 22, Name = "Michigan", CountryID = 2 },
                new State { ID = 23, Name = "Minnesota", CountryID = 2 },
                new State { ID = 24, Name = "Mississippi", CountryID = 2 },
                new State { ID = 25, Name = "Missouri", CountryID = 2 },
                new State { ID = 26, Name = "Montana", CountryID = 2 },
                new State { ID = 27, Name = "Nebraska", CountryID = 2 },
                new State { ID = 28, Name = "Nevada", CountryID = 2 },
                new State { ID = 29, Name = "New Hampshire", CountryID = 2 },
                new State { ID = 30, Name = "New Jersey", CountryID = 2 },
                new State { ID = 31, Name = "New Mexico", CountryID = 2 },
                new State { ID = 32, Name = "New York", CountryID = 2 },
                new State { ID = 33, Name = "North Carolina", CountryID = 2 },
                new State { ID = 34, Name = "North Dakota", CountryID = 2 },
                new State { ID = 35, Name = "Ohio", CountryID = 2 },
                new State { ID = 36, Name = "Oklahoma", CountryID = 2 },
                new State { ID = 37, Name = "Oregon", CountryID = 2 },
                new State { ID = 38, Name = "Pennsylvania", CountryID = 2 },
                new State { ID = 39, Name = "Rhode Island", CountryID = 2 },
                new State { ID = 40, Name = "South Carolina", CountryID = 2 },
                new State { ID = 41, Name = "South Dakota", CountryID = 2 },
                new State { ID = 42, Name = "Tennessee", CountryID = 2 },
                new State { ID = 43, Name = "Texas", CountryID = 2 },
                new State { ID = 44, Name = "Utah", CountryID = 2 },
                new State { ID = 45, Name = "Vermont", CountryID = 2 },
                new State { ID = 46, Name = "Virginia", CountryID = 2 },
                new State { ID = 47, Name = "Washington", CountryID = 2 },
                new State { ID = 48, Name = "West Virginia", CountryID = 2 },
                new State { ID = 49, Name = "Wisconsin", CountryID = 2 },
                new State { ID = 50, Name = "Wyoming", CountryID = 2 }
            };

            context.States.AddOrUpdate(s => s.ID, usStates.ToArray());

            // Seed Provinces for Canada
            var canadianProvinces = new List<State>
            {
                new State { ID = 101, Name = "Alberta", CountryID = 1 },
                new State { ID = 102, Name = "British Columbia", CountryID = 1 },
                new State { ID = 103, Name = "Manitoba", CountryID = 1 },
                new State { ID = 104, Name = "New Brunswick", CountryID = 1 },
                new State { ID = 105, Name = "Newfoundland and Labrador", CountryID = 1 },
                new State { ID = 106, Name = "Nova Scotia", CountryID = 1 },
                new State { ID = 107, Name = "Ontario", CountryID = 1 },
                new State { ID = 108, Name = "Prince Edward Island", CountryID = 1 },
                new State { ID = 109, Name = "Quebec", CountryID = 1 },
                new State { ID = 110, Name = "Saskatchewan", CountryID = 1 },
                new State { ID = 111, Name = "Northwest Territories", CountryID = 1 },
                new State { ID = 112, Name = "Nunavut", CountryID = 1 },
                new State { ID = 113, Name = "Yukon", CountryID = 1 }
            };

            context.States.AddOrUpdate(p => p.ID, canadianProvinces.ToArray());

            context.SaveChanges();
            System.Diagnostics.Debug.WriteLine("Seeding completed.");
        }
    }
}
