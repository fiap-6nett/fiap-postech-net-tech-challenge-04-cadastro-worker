using Contato.Cadastrar.Worker.Application.Interfaces;
using Contato.Cadastrar.Worker.Application.Services;
using Contato.Cadastrar.Worker.Domain.Interfaces;
using Contato.Cadastrar.Worker.Infra.Mensageria;
using Contato.Cadastrar.Worker.Infra.Mensageria.Consumer;
using Contato.Cadastrar.Worker.Infra.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // MongoDB
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        // RabbitMQ
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));

        services.AddSingleton<IConnection>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password,
                VirtualHost = settings.VirtualHost
            };

            return factory.CreateConnection();
        });

        // Injeção de dependências da aplicação
        services.AddSingleton<IContatoRepository, ContatoRepository>();
        services.AddSingleton<IContatoAppService, ContatoAppService>();
        services.AddSingleton<IContatoConsumer, ContatoConsumer>();

        // Worker
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
