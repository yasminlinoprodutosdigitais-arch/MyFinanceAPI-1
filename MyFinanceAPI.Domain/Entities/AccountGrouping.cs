using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class AccountGrouping
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? SubCategory { get; set; }
    public List<Account>? Accounts { get; set; }
    public List<Category>? Categories { get; set; }
    public List<MonthlyUpdate> MonthlyUpdate { get; set; }

    public AccountGrouping(int categoryId, string categoryName, string subCategory, List<Account> accounts)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        SubCategory = subCategory;
        Accounts = accounts;
    }
}
