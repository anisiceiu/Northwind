
using Northwind.Application.Interfaces;
using Northwind.Application.Services;
using Northwind.Core.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Infrastructure.Data;
using Northwind.Infrastructure.Repositories;
using Northwind.Infrastructure.Extensions;

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

            builder.Services.AddMappingProfiles();
            builder.Services.AddServices();

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
