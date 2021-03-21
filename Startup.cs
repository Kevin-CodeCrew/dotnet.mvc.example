using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using test_mvc_webapp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using System.IO;
using test_mvc_webapp.Helper;
using Microsoft.AspNetCore.Mvc;

namespace test_mvc_webapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));


            // services.AddDbContext<MvcWebAppDbContext>(options =>
            //     options.UseSqlite(
            //         Configuration.GetConnectionString("DefaultConnection")));


            // services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //     .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //////////////////////////////////////////////////////////////////////////////////
            services.AddControllersWithViews(); // ONLY THING IN THIS METHOD WITH NEW PROJECT
                                                //////////////////////////////////////////////////////////////////////////////////

            services.AddRazorPages();

            // Wire in file system provider for storing images
            // TODO #1 Re-evaluate if singleton needed
            services.AddSingleton<IFileProvider>(
                 new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img"))
                 );

            // Setup auth policy support to control what different roles see
            services.AddMvc(obj =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // Turn on require confirmation on registration
            services.Configure<IdentityOptions>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 8;
                opts.SignIn.RequireConfirmedEmail = true;
            });

            services.AddTransient<EmailHelper>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);  
        }

        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // SETUP ROLES 
            CreateUserRoles(services).Wait();
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var testChkRole = RoleManager.FindByNameAsync("admin");
            IdentityResult roleResult;
            //Add Admin Role
            var adminRoleCheck = await RoleManager.RoleExistsAsync("admin");
            if (!adminRoleCheck)
            {
                //create the admin role
                roleResult = await RoleManager.CreateAsync(new IdentityRole("admin"));
            }

            //Add User Role
            var userRoleCheck = await RoleManager.RoleExistsAsync("user");
            if (!userRoleCheck)
            {
                //create the user role
                roleResult = await RoleManager.CreateAsync(new IdentityRole("user"));
            }
            //Add User Role
            var testRoleCheck = await RoleManager.RoleExistsAsync("test");
            if (!testRoleCheck)
            {
                //create the test role
                roleResult = await RoleManager.CreateAsync(new IdentityRole("test"));
            }            
        }
    }
}
