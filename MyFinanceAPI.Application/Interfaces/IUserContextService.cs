namespace MyFinanceAPI.Application.Interfaces;

public interface IUserContextService
{
    int? GetUserIdFromClaims();
}
