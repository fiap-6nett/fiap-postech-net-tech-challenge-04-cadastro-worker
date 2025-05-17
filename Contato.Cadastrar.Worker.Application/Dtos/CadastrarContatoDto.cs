using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Contato.Cadastrar.Worker.Application.Dtos;

public class CadastrarContatoDto
{
    [Required(ErrorMessage = "Id é obrigatório.")]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres.")]
    [RegularExpression(@"^[A-Za-zÀ-ÿ\s]+$", ErrorMessage = "Nome deve conter apenas letras e espaços.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Telefone é obrigatório.")]
    [RegularExpression(@"^\d{8,9}$", ErrorMessage = "Telefone deve conter 8 ou 9 dígitos numéricos.")]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "DDD é obrigatório.")]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "DDD deve conter exatamente 2 dígitos.")]
    public string Ddd { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}