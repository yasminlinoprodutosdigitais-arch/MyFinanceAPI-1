using System;

namespace MyFinanceAPI.Application.Interfaces;
public interface IValidadorToken
{
    public bool ValidarTokenPorUsuario(string token);
}
