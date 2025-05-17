using Contato.Cadastrar.Worker.Domain.Entities;

namespace Contato.Cadastrar.Worker.Domain.Interfaces;

public interface IContatoRepository
{
    public ContatoEntity ObterPorID(Guid id);
    public void CadastrarContato(ContatoEntity contato);
    
}