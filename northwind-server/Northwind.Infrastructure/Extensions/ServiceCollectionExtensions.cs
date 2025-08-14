using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Application.Interfaces;
using Northwind.Application.MappingProfiles;
using Northwind.Application.Services;
using Northwind.Core.Interfaces;
using Northwind.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryProfile>();
                // Add more profiles here if needed
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Specific service
            services.AddScoped<ICategoryService, CategoryService>();
            return services;
        }
    }
}
