
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pedalacom.Authentication;
using Pedalacom.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace Pedalacom
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(
                    "CarsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    );
            });

            builder.Services.AddControllers().AddJsonOptions(
                x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddDbContext<AdventureWorks2019Context>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("PedalacomDB")));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            builder.Services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            builder.Services.AddAuthorization(opt 
                => opt.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build()));

            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
            builder.Services.AddAuthorization();
            // Add configuration from appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.AddSingleton<JwtManager>(provider 
                => { var jwtService = new JwtManager(builder.Configuration["Jwt:Key"], 
                                                     builder.Configuration["Jwt:Issuer"], 
                                                     builder.Configuration["Jwt:Audience"]); return jwtService; });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseCors("CarsPolicy");
            app.UseAuthentication();

            app.UseAuthorization();
            IConfiguration configuration = app.Configuration;
            IWebHostEnvironment environment = app.Environment;

            app.MapControllers();

            app.Run();
        }
    }
}