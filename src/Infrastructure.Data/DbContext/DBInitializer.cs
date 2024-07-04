using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.DbContext
{
    public class DbInitializer
    {
        //public static async Task SeedRoleData(IServiceProvider serviceProvider)
        //{
        //    var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        //    var roles = new List<Role>
        //    {
        //        new Role
        //        {
        //            ConcurrencyStamp = Guid.NewGuid().ToString(),
        //            Name = ERole.USER.ToString()
        //        },
        //        new Role
        //        {
        //            ConcurrencyStamp = Guid.NewGuid().ToString(),
        //            Name = ERole.ADMIN.ToString(),
        //        }
        //    };

        //    if (!roleManager.Roles.Any())
        //    {
        //        foreach (var role in roles)
        //        {
        //            if (!await roleManager.RoleExistsAsync(role.Name))
        //                await roleManager.CreateAsync(role);
        //        }
        //    }
        //}

        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();

            var adminUser = new User
            {
                Email = "richkeed94@gmail.com",
                PhoneNumber = "070Wema945",
                StateId = 1,
                LGAId = 1,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.Now,
            };
            adminUser.IsVerified = true;
            adminUser.UserName = adminUser.Email;

            string password = "Password@123";

            logger.LogInformation($"Attempting to create admin user with Email: {adminUser.Email}, UserName: {adminUser.UserName}, Id: {adminUser.Id}");

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user is null)
            {
                if (string.IsNullOrWhiteSpace(adminUser.Email) || !IsValidEmail(adminUser.Email))
                {
                    var errorMessage = $"Invalid email format for admin user: {adminUser.Email}";
                    logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                // Log the password before creation
                logger.LogInformation($"Creating admin user with password: '{password}'");

                var userCreationResult = await userManager.CreateAsync(adminUser, password);
                if (!userCreationResult.Succeeded)
                {
                    var errors = string.Join(", ", userCreationResult.Errors.Select(e => e.Description));
                    logger.LogError($"Failed to create admin user: {errors}");
                    throw new Exception($"Failed to create admin user: {errors}");
                }
            }
            else
            {
                adminUser = user;
                logger.LogInformation("Admin user already exists.");
            }

            var roles = new List<string>()
            {
                new(ERole.USER.ToString()),
                new(ERole.ADMIN.ToString())
            };

            foreach (var role in roles)
            {
                // Check if the role exists before adding to the user
                if (await roleManager.RoleExistsAsync(role))
                {
                    // Check if the user already has the role
                    if (!await userManager.IsInRoleAsync(adminUser, role))
                    {
                        var roleResult = await userManager.AddToRoleAsync(adminUser, role);
                        if (!roleResult.Succeeded)
                        {
                            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                            logger.LogError($"Failed to assign role '{role}' to admin user: {errors}");
                            throw new Exception($"Failed to assign role '{role}' to admin user: {errors}");
                        }
                        else
                        {
                            logger.LogInformation($"Role '{role}' assigned to admin user successfully.");
                        }
                    }
                    else
                    {
                        logger.LogInformation($"Admin user already has the role '{role}'.");
                    }
                }
                else
                {
                    logger.LogWarning($"Role '{role}' does not exist in the database.");
                }
            }
        }
        private static bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}