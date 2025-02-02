using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Application.DTO;

public class RefreshTokenDto
{
    [Required (ErrorMessage = "O Token é obrigatório.")]
    [MinLength(10, ErrorMessage = "Token inválido.")]
    public string Token { get; set; }
    
    [Required (ErrorMessage = "O TokenRefresh é obrigatório.")]
    public string TokenRefresh { get; set; }
}