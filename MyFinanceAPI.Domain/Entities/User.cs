using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities
{
    public class User : IdentityUser
    {
        public string? Login { get; set; } // Propriedade pública para facilitar a modificação

        public string? NomeUsuario { get; set; } 
    }
}
