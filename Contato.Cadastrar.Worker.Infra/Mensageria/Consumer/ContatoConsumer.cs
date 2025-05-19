using Contato.Cadastrar.Worker.Application.Interfaces;
using Contato.Cadastrar.Worker.Infra.Mensageria.Consumer;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using Contato.Cadastrar.Worker.Application.Dtos;

public class ContatoConsumer : IContatoConsumer, IDisposable
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly IContatoAppService _appService;
    private readonly string _queueName;
    private bool _consumingStarted = false;
    private EventingBasicConsumer? _consumer;

    public ContatoConsumer(IContatoAppService appService, IConfiguration configuration, IConnection rabbitConnection)
    {
        _appService = appService;
        _connection = rabbitConnection;
        _channel = _connection.CreateModel();

        _queueName = configuration["RabbitMQ:QueueName"] ?? "cadastro-contato";

        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Define que só uma mensagem por vez será entregue ao consumidor
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    public void StartConsuming(CancellationToken cancellationToken)
    {
        if (_consumingStarted)
            return;

        _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Mensagem recebida: {message}");

            var dto = JsonConvert.DeserializeObject<CadastrarContatoDto>(message);

            _appService.CadastrarContato(dto);
        };

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: _consumer);

        _consumingStarted = true;
    }

    public void Dispose()
    {
        try
        {
            _consumer?.Model?.Close();
            _channel?.Close();
            _connection?.Close();
        }
        catch
        {
            // Evita exceções se já estiver fechado
        }
    }
}
