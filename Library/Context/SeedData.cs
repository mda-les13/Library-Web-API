using Library.DataAccess.Entities;

namespace Library.DataAccess.Context
{
    public static class SeedData
    {
        public static async Task InitializeAsync(LibraryContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
