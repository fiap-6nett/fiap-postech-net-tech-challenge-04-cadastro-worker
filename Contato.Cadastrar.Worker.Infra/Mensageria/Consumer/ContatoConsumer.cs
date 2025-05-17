using System.Text;
using Contato.Cadastrar.Worker.Application.Dtos;
using Contato.Cadastrar.Worker.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Contato.Cadastrar.Worker.Infra.Mensageria.Consumer;

public class ContatoConsumer : IContatoConsumer, IDisposable
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly IContatoAppService _appService;

    public ContatoConsumer(IContatoAppService appService, IConfiguration configuration, IConnection rabbitConnection)
    {
        _appService = appService;
        _connection = rabbitConnection;
        _channel = _connection.CreateModel();

        var queueName = configuration["RabbitMQ:QueueName"] ?? "atualizacao-contato";

        _channel.QueueDeclare(queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void StartConsuming(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Mensagem recebida: {message}");

            var dto = JsonConvert.DeserializeObject<CadastrarContatoDto>(message);

            _appService.CadastrarContato(dto);
        };

        
        _channel.BasicConsume(queue: "cadastro-contato", autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}