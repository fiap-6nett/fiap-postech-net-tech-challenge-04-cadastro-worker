using Contato.Cadastrar.Worker.Application.Dtos;
using Contato.Cadastrar.Worker.Application.Interfaces;
using Contato.Cadastrar.Worker.Domain.Entities;
using Contato.Cadastrar.Worker.Domain.Interfaces;

namespace Contato.Cadastrar.Worker.Application.Services;

public class ContatoAppService : IContatoAppService
{
    private readonly IContatoRepository _contatoRepository;
    
    public ContatoAppService(IContatoRepository contatoRepository)
    {
        _contatoRepository = contatoRepository;
    }
    
    public Task CadastrarContato(CadastrarContatoDto dto)
    {
       
        var contato = new ContatoEntity();
        
        contato.SetId(dto.Id);
        contato.SetNome(dto.Nome);
        contato.SetEmail(dto.Email);
        contato.SetTelefone(dto.Telefone);
        contato.SetDdd(dto.Ddd);
        
        _contatoRepository.CadastrarContato(contato);
        
        return Task.CompletedTask;

    }
}