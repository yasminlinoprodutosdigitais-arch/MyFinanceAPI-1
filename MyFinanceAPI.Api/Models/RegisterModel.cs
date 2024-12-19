using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Api.Models;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "A senha precisa ter pelo menos 6 caracteres.")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password doesn't match")]
    public string ConfirmPassword { get; set; }
}
