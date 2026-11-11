using System.Text.Json;
using MyFinanceAPI.Application.DTO.Notificacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>Card #73: preferências gravadas como jsonb, uma linha por usuário; o que faltar vale o padrão.</summary>
public class PreferenciaNotificacaoService(IPreferenciaNotificacaoRepository repository) : IPreferenciaNotificacaoService
{
    public static readonly int[] OpcoesContasAVencer = [0, 1, 2, 3];
    public static readonly int[] OpcoesFatura = [1, 3, 5];
    public static readonly int[] OpcoesAgenda = [0, 1];
    public static readonly int[] OpcoesAcertos = [15, 30, 60];

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<PreferenciasNotificacaoDTO> ObterAsync(int userId) =>
        Ler((await repository.ObterAsync(userId))?.Config);

    public async Task<PreferenciasNotificacaoDTO> SalvarAsync(int userId, PreferenciasNotificacaoDTO dto)
    {
        var limpo = ComPadroes(dto);
        Validar(limpo);
        await repository.SalvarConfigAsync(userId, JsonSerializer.Serialize(limpo, Json));
        return limpo;
    }

    /// <summary>Lê o jsonb salvo; vazio, inválido ou com partes faltando volta com os padrões.</summary>
    public static PreferenciasNotificacaoDTO Ler(string? config)
    {
        if (string.IsNullOrWhiteSpace(config))
            return new();
        try
        {
            return ComPadroes(JsonSerializer.Deserialize<PreferenciasNotificacaoDTO>(config, Json));
        }
        catch (JsonException)
        {
            return new();
        }
    }

    private static PreferenciasNotificacaoDTO ComPadroes(PreferenciasNotificacaoDTO? dto)
    {
        var padrao = new PreferenciasNotificacaoDTO();
        if (dto == null)
            return padrao;
        dto.ContasAVencer ??= padrao.ContasAVencer;
        dto.Vencidas ??= padrao.Vencidas;
        dto.Fatura ??= padrao.Fatura;
        dto.Agenda ??= padrao.Agenda;
        dto.Acertos ??= padrao.Acertos;
        return dto;
    }

    private static void Validar(PreferenciasNotificacaoDTO dto)
    {
        Exigir(OpcoesContasAVencer, dto.ContasAVencer.DiasAntes, "Antecedência de contas a vencer");
        Exigir(OpcoesFatura, dto.Fatura.DiasAntes, "Antecedência da fatura");
        Exigir(OpcoesAgenda, dto.Agenda.DiasAntes, "Antecedência da agenda");
        Exigir(OpcoesAcertos, dto.Acertos.DiasEmAberto, "Dias em aberto dos acertos");
    }

    private static void Exigir(int[] opcoes, int valor, string campo)
    {
        if (!opcoes.Contains(valor))
            throw new RegraNegocioException($"{campo} deve ser {string.Join(", ", opcoes[..^1])} ou {opcoes[^1]}.");
    }
}
