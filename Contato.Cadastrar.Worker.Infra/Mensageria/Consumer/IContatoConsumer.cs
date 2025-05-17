namespace Contato.Cadastrar.Worker.Infra.Mensageria.Consumer;

public interface IContatoConsumer
{
    void StartConsuming(CancellationToken cancellationToken);
}