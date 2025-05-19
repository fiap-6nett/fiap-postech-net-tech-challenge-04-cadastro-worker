using Contato.Cadastrar.Worker.Domain.Entities;
using Contato.Cadastrar.Worker.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Contato.Cadastrar.Worker.Infra.Repositories;

public class ContatoRepository : IContatoRepository
{
    private readonly IMongoCollection<ContatoEntity> _contatos;

    public ContatoRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> mongoDbSettings)
    {
        var database = mongoClient.GetDatabase(mongoDbSettings.Value.Database);
        _contatos = database.GetCollection<ContatoEntity>("Contatos"); 
    }
    
    public ContatoEntity ObterPorID(Guid id)
    {
        return  _contatos.Find(c => c.Id == id).FirstOrDefault();
    }

    public void CadastrarContato(ContatoEntity contato)
    {
        try
        {
            
            // Criando um filtro para buscar pelo Id
            var filterId = Builders<ContatoEntity>.Filter.Eq(c => c.Id, contato.Id);
            var filterEmail = Builders<ContatoEntity>.Filter.Eq(c => c.Email, contato.Email);

            // Realizando a busca no banco
            var existingContato =  _contatos.Find(filterId).FirstOrDefault();
            

            if (existingContato == null)
            {
                throw new Exception("Contato não encontrado.");
            }
            
            var filter = Builders<ContatoEntity>.Filter.Eq(c => c.Id, contato.Id);
            var update = Builders<ContatoEntity>.Update
                .Set(c => c.Nome, contato.Nome)
                .Set(c => c.Telefone, contato.Telefone)
                .Set(c => c.Email, contato.Email)
                .Set(c => c.Ddd, contato.Ddd);

            _contatos.UpdateOneAsync(filter, update);
        }
        catch (Exception ex)
        {
            throw new Exception($"Falha ao atualizar o contato. Erro {ex.Message}");
        }
       
    }
}