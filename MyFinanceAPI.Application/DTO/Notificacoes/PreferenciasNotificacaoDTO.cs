namespace MyFinanceAPI.Application.DTO.Notificacoes;

/// <summary>
/// Preferências de notificação (card #73). Os valores iniciais são os padrões de quem nunca salvou.
/// Gravado como jsonb (preferencias_notificacao.config); campos novos (e-mail, Fase 2) entram aqui sem migration.
/// </summary>
public class PreferenciasNotificacaoDTO
{
    public AvisoComAntecedenciaDTO ContasAVencer { get; set; } = new() { Ativo = true, DiasAntes = 1 };
    public AvisoVencidasDTO Vencidas { get; set; } = new();
    public AvisoComAntecedenciaDTO Fatura { get; set; } = new() { Ativo = true, DiasAntes = 3 };
    public AvisoComAntecedenciaDTO Agenda { get; set; } = new() { Ativo = true, DiasAntes = 0 };
    public AvisoAcertosDTO Acertos { get; set; } = new();
    /// <summary>"Pausar tudo" (férias, viagens): até este dia (inclusive) o sino fica sem avisos. Null = não pausado.</summary>
    public DateOnly? PausadoAte { get; set; }
}

public class AvisoComAntecedenciaDTO
{
    public bool Ativo { get; set; } = true;
    /// <summary>Quantos dias antes o aviso aparece (0 = no dia).</summary>
    public int DiasAntes { get; set; }
}

public class AvisoVencidasDTO
{
    public bool Ativo { get; set; } = true;
    /// <summary>Só as vencidas há até 90 dias entram no aviso (as antigas ficam de fora).</summary>
    public bool SoUltimos90Dias { get; set; } = true;
}

public class AvisoAcertosDTO
{
    public bool Ativo { get; set; } = true;
    /// <summary>Avisa de quem deve há mais que isso (a partir da compra mais antiga não quitada).</summary>
    public int DiasEmAberto { get; set; } = 30;
}
