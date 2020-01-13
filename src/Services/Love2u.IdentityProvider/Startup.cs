using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Love2u.IdentityProvider.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Love2u.IdentityProvider.Data.Models;
using System.Reflection;
using Love2u.IdentityProvider.Services;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Love2u.IdentityProvider.Extensions;

namespace Love2u.IdentityProvider
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            services.AddDbContext<Love2uIdentityContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDefaultIdentity<User>(options => 
            {
                options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
            }).AddEntityFrameworkStores<Love2uIdentityContext>();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, ClaimsPrincipalFactory>();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                });

            if (Environment.IsDockerHosted()) 
            {
                var redis = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection"));
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "Love2u.IdentityProvider";
                })
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }

            services.AddAuthentication();
            services.AddMvc()
                .AddRazorPagesOptions(options => 
                    options.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account", model =>
                    {
                        foreach (var selector in model.Selectors)
                        {
                            var attributeRouteModel = selector.AttributeRouteModel;
                            attributeRouteModel.Order = -1;
                            attributeRouteModel.Template = attributeRouteModel.Template.Remove(0, "Identity".Length);
                        }
                    })).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddRazorPages();
            services.AddControllers();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddCors(options =>
            {
                options.AddPolicy("Love2u.Angular", policyBuilder =>
                {
                    policyBuilder.WithOrigins(this.Configuration["ANGULAR_SPA_ORIGIN"])
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseCors("Love2u.Angular");
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
