﻿using FluentValidation;
using Ticketing.Command.Application.Core;
using Ticketing.Command.Application.Models;

namespace Ticketing.Command.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection(nameof(MongoSettings)));
            services.Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly));
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            
            return services;
        }
    }
}
