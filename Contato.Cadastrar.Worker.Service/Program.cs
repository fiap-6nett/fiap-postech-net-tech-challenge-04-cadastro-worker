using Contato.Cadastrar.Worker.Application.Interfaces;
using Contato.Cadastrar.Worker.Application.Services;
using Contato.Cadastrar.Worker.Domain.Interfaces;
using Contato.Cadastrar.Worker.Infra.Mensageria;
using Contato.Cadastrar.Worker.Infra.Mensageria.Consumer;
using Contato.Cadastrar.Worker.Infra.Repositories;
using Contato.Cadastrar.Worker.Service;
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
        
        
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var connectionString = mongoDbSettings.ConnectionString;
            return new MongoClient(connectionString);
        });
        
        // Carregar as configurações de RabbitMQ
        var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();
        Console.WriteLine($"RabbitMQ HostName: {rabbitMqSettings.HostName}");  // Verifique se os valores estão corretos no console
        
        // Registra a configuração de RabbitMqSettings
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        
        
        
        // Registra o IConnection usando as configurações
        services.AddSingleton<IConnection>(sp =>
        {
            var rabbitMqSettings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
            Console.WriteLine($"RabbitMQ HostName: {rabbitMqSettings.HostName}"); // Verifique novamente
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqSettings.HostName,
                UserName = rabbitMqSettings.UserName,
                Password = rabbitMqSettings.Password,
                VirtualHost = rabbitMqSettings.VirtualHost
            };

            return factory.CreateConnection();
        });


       
        services.AddSingleton<IContatoRepository, ContatoRepository>();
        services.AddSingleton<IContatoAppService, ContatoAppService>();
        services.AddSingleton<IContatoConsumer, ContatoConsumer>();
        
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();