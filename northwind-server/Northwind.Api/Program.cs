
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Api.Configuration;
using Northwind.Application.Interfaces;
using Northwind.Application.Services;
using Northwind.Core.Interfaces;
using Northwind.Infrastructure.Data;
using Northwind.Infrastructure.Extensions;
using Northwind.Infrastructure.Repositories;

namespace Northwind.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Add services to the container.
            builder.Services.AddDbContext<NorthwindContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("NorthwindConnection")));

            var appSettingsSection = builder.Configuration.GetSection("AppSettings");
            builder.Services.Configure<AppSettings>(appSettingsSection);

            builder.Services.AddMappingProfiles();
            builder.Services.AddApplicationServices();

            // Add this using directive
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
