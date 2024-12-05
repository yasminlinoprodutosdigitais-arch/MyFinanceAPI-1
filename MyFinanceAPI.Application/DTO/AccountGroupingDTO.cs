using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class AccountGroupingDTO
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? SubCategory { get; set; }
    public List<Account>? Accounts { get; set; }
}
