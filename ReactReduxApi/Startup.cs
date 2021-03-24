using CommonLib;
using DbRepository;
using DbRepository.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ReactReduxApi.Helpers;
using System;
using System.Text;

namespace ReactReduxApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsts(config => config.Preload = true);
            services.AddHttpsRedirection(config =>
            {
                config.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                config.HttpsPort = _configuration.GetValue<int>("HttpsPort");
            });
            services.AddCors(config => config.AddDefaultPolicy(new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy()
            {
                IsOriginAllowed = (a) => { return true; },// allow all origin
                SupportsCredentials = true               
            }));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(builder=>
                {
                    builder.SaveToken = true;
                    builder.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:EncriptionKey"])),
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ClockSkew = TimeSpan.Zero
                    };
                    builder.ClaimsIssuer = _configuration["Jwt:Issuer"];   
                });
            services.AddAuthorization(policies => {
                policies.AddPolicy("UserId", policy => policy.RequireClaim("UserId"));
             });
            services.AddMvc(config=> config.EnableEndpointRouting = false);
            services.AddScoped<IRepositoryContextFactory, UsersRepositoryContextFactory>();
            services.AddScoped<IUsersRepository>(provider =>
                new UsersInMemoryRepository(_configuration.GetConnectionString("default"),
                    provider.GetService<IRepositoryContextFactory>()));
            services.AddAutoMapper(typeof(AutomapperProfile));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
                app.UseHsts();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                await next();
            });
            
            app.UseHttpsRedirection();
            app.UseCors(builder=> builder.WithOrigins(_configuration["Origins"].Split(","))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            ); //temporary in dev

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = HttpOnlyPolicy.None,
                Secure = CookieSecurePolicy.Always
            });
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = context.Request.Cookies[_configuration[Constants.JwtCookieToken]];
                    if (!string.IsNullOrEmpty(token))
                        context.Request.Headers.Add("Authorization", "Bearer " + token);
                }
                
                await next();
            });
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MigrateDatabase(_configuration);
            app.UseMvc(routes=>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });            
        }        
    }
}
