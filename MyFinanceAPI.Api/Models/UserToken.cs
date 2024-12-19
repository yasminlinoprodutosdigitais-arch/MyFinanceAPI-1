using System;

namespace MyFinanceAPI.Api.Controllers.Autenticacao;

public class UserToken
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}

