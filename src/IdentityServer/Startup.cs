using System.Linq;
using System.Reflection;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityServer4;trusted_connection=yes;";


            /*  AddDeveloperSigningCredential:

                Not recommended for production - you need to store your key material somewhere secure.

                At first startup, IdentityServer will create a developer signing key for you, it’s a file called tempkey.rsa. 
                You don’t have to check that file into your source control, it will be re-created if it is not present.


                IdentityServer4.EntityFramework implements the required stores and services using the following DbContext’s:
               
                ConfigurationDbContext - used for configuration data such as clients, resources, and scopes
                PersistedGrantDbContext - used for temporary operational data such as authorization codes, and refresh tokens.

                Because we are using EF migrations in this quickstart, the call to MigrationsAssembly is used to inform Entity Framework 
                that the host project will contain the migrations code. This is necessary since the host project is in a different assembly 
                than the one that contains the DbContext classes.
             */
            services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    })
                .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    });

            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "271038709994-ah2rld207ourmj1qu2u62it8j9bovgn0.apps.googleusercontent.com";
                    options.ClientSecret = "KES9pTgtacMbzz5KstgPdHtU";
                });

            services.AddAuthentication()
                .AddFacebook("Facebook", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "595912740568242";
                    options.ClientSecret = "a0a98667147851ba8dbaff1b71a9a019";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
