using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Teletext.Areas.Identity.Data;
using Teletext.Helpers;
using Teletext.Models;
using Teletext.Services;

namespace Teletext
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<TeletextContext>(options =>
                options.UseSqlServer(connectionString, x => x.UseDateOnlyTimeOnly()));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<TeletextUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<TeletextContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ITeletextRepository, TeletextRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();


            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }


            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TeletextUser>>();
                
                string email = "admin@email.com";
                string password = "DontForgetThis=123";

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new TeletextUser();
                    user.UserName = email;
                    user.Email = email;
                    user.EmailConfirmed = true;
                    
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");


                }
                
                var repo = scope.ServiceProvider.GetService<ITeletextRepository>();           
                var populator = new DbPopulator(repo);
                await populator.CreateData();
            }

            app.Run();
        }
    }
}