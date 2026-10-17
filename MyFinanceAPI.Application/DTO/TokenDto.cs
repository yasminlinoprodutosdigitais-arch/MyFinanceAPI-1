using System;

namespace MyFinanceAPI.Application.DTO;

public sealed record TokenDto(object Token, DateTime DataCriacao, DateTime DataExpiracao);
