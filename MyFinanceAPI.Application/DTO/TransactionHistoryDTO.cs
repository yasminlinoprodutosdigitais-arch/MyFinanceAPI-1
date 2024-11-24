using System;
using System.ComponentModel;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class TransactionHistoryDTO
{
    public int Id { get; set; }

    [DisplayName("Mounth")]
    public DateTime Mouth { get; set; }

    [DisplayName("Name")]
    public string Name { get; set; }

    [DisplayName("Value")]
    public double Value { get; set; }

    [DisplayName("IdCategory")]
    public int IdCategory { get; set; }

    [DisplayName("Status")]
    public string Status { get; set; }
    
}
