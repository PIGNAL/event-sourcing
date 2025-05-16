﻿using Common.Core.Events;
using Common.Core.Producers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Infrastructure.EventsSourcing;
using Ticketing.Command.Infrastructure.Persistence;
using Ticketing.Command.Infrastructure.Repositories;

namespace Ticketing.Command.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) {

            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<TicketCreatedEvent>();
            BsonClassMap.RegisterClassMap<TicketUpdatedEvent>();
            BsonClassMap.RegisterClassMap<TicketDeletedEvent>();
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.AddTransient<IEventModelRepository, EventModelRepository>();
            services.AddSingleton<IMongoClient, MongoClient>(s => new MongoClient(configuration.GetConnectionString("MongoDb")));
            services.AddTransient<IEventStore, EventStore>();
            services.AddTransient<IEventSourcingHandler<TicketAggregate>, TicketingEventSourcingHandler>();
            services.AddScoped<IEventProducer, TicketEventProducer>();
            return services;
        }
    }
}
