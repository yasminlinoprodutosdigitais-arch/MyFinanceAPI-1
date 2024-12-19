using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Api.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid format email")]
    public string UserName  { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, ErrorMessage = "The {0} must be at leeast {2} and at max " + "{1} chatacters long.", MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
