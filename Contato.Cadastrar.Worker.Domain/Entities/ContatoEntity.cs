using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Contato.Cadastrar.Worker.Domain.Entities;

public class ContatoEntity
{
    [BsonId]
    public Guid Id { get; private set; }
    public string Nome {get; private set;}
    public string Telefone { get; private set; }
    public string Email{get; private set;}
    public string Ddd { get; private set; }

    public ContatoEntity()
    {
        
    }

    public ContatoEntity(string nome, string telefone, string email, string ddd)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Ddd = ddd;
    }

    #region  validações

    public void SetId(Guid id)
    {
        try
        {
            Id = id;
        }
        catch
        {
            throw new ArgumentException("Id invalido");
        }
        
    }
    public void SetNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ValidationException("Nome cannot be empty");
        
        if (nome.Length < 3)
            throw new ValidationException("Nome must be at least 3 characters long");
       
        Nome = nome;
    }
    
    public void SetTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ValidationException("Telefone cannot be empty");

        if (telefone.Length != 8 && telefone.Length != 9)
            throw new ValidationException("Telefone must be 8 or 9 digits long");

        if (!telefone.All(char.IsDigit))
            throw new ValidationException("Telefone must contain only numbers");

        Telefone = telefone;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email cannot be empty");

        try
        {
            var addr = new MailAddress(email);
            if (addr.Address != email)
                throw new ValidationException("Invalid email format");
        }
        catch
        {
            throw new ValidationException("Invalid email format");
        }

        Email = email;
    }

    public void SetDdd(string ddd)
    {
        if(string.IsNullOrWhiteSpace(ddd))
            throw new ValidationException("DDD cannot be empty");
        
        if (ddd.Length != 2)
            throw new ValidationException("DDD must be 2 characters");
        
        if (!ddd.All(char.IsDigit))
            throw new ValidationException("DDD must contain only numbers");
        
        Ddd = ddd;
    }

    #endregion

}
