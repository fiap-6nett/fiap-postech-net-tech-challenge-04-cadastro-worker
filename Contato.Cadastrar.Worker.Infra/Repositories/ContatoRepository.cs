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
        _contatos = database.GetCollection<ContatoEntity>("contatos"); 
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
            var existingContato_id =  _contatos.Find(filterId).FirstOrDefault();
            var existingContato_email =  _contatos.Find(filterEmail).FirstOrDefault();
            
            if (existingContato_id == null && existingContato_email == null)
            {
                _contatos.InsertOneAsync(contato);
            }
           
        }
        catch (Exception ex)
        {
            throw new Exception($"Falha ao inserir o contato. Erro {ex.Message}");
        }
       
    }
}