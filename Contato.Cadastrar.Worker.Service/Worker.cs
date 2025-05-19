using Contato.Cadastrar.Worker.Infra.Mensageria.Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IContatoConsumer _consumer;

    public Worker(ILogger<Worker> logger, IContatoConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando worker e consumidor da fila...");

        _consumer.StartConsuming(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        _logger.LogInformation("Encerrando worker e fechando consumidor da fila.");
        (_consumer as IDisposable)?.Dispose();
        base.Dispose();
    }
}
