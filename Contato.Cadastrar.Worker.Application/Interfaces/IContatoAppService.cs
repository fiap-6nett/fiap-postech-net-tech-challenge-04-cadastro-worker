using Contato.Cadastrar.Worker.Application.Dtos;

namespace Contato.Cadastrar.Worker.Application.Interfaces;

public interface IContatoAppService
{
     Task CadastrarContato(CadastrarContatoDto dto);
}