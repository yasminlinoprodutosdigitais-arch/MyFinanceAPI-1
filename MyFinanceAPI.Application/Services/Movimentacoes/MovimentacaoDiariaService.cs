using AutoMapper;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Lançamento com banco e tipo de cartão mexe no saldo do banco: criar aplica, excluir desfaz
/// e editar desfaz o antigo e aplica o novo — vale também para troca de tipo (Desconto/Crédito) e de banco.
/// </summary>
public class MovimentacaoDiariaService(IMovimentacaoDiariaRepository movimentacaoDiariaRepository, IBancoRepository bancoRepository, IMapper mapper)
    : ServicoCadastro<MovimentacaoDiaria, MovimentacaoDiariaDTO>(movimentacaoDiariaRepository, mapper), IMovimentacaoDiariaService
{
    /// <summary>O que o lançamento faz no saldo, copiado antes de gravar (a entidade rastreada muda no Atualizar).</summary>
    private sealed record Efeito(int? BancoId, int? TipoCartaoId, decimal Valor, string? TipoLancamento)
    {
        public bool MexeNoSaldo => BancoId is > 0 && TipoCartaoId != null;
        public decimal NoSaldo => TipoLancamento == "Desconto" ? -Valor : Valor;
    }

    public async Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoByDate(DateTime date, int userId) =>
        Mapper.Map<IEnumerable<MovimentacaoDiariaDTO>>(await movimentacaoDiariaRepository.GetMovimentacaoByDate(date, userId));

    public override async Task AdicionarAsync(MovimentacaoDiariaDTO dto, int userId)
    {
        await base.AdicionarAsync(dto, userId);
        await Aplicar(Do(dto), userId);
    }

    public override async Task AtualizarAsync(MovimentacaoDiariaDTO dto, int userId)
    {
        var anterior = await EfeitoGravado(dto.Id, userId);

        await base.AtualizarAsync(dto, userId);

        if (anterior != null)
            await Desfazer(anterior, userId);
        await Aplicar(Do(dto), userId);
    }

    public override async Task RemoverAsync(int id, int userId)
    {
        var anterior = await EfeitoGravado(id, userId);

        await base.RemoverAsync(id, userId);

        if (anterior != null)
            await Desfazer(anterior, userId);
    }

    private static Efeito Do(MovimentacaoDiariaDTO dto) => new(dto.BancoId, dto.TipoCartaoId, dto.Valor, dto.TipoLancamento);

    private async Task<Efeito?> EfeitoGravado(int id, int userId)
    {
        var gravada = await movimentacaoDiariaRepository.ObterPorIdAsync(id, userId);
        return gravada == null ? null : new(gravada.BancoId, gravada.TipoCartaoId, gravada.Valor, gravada.TipoLancamento);
    }

    private Task Aplicar(Efeito efeito, int userId) =>
        efeito.MexeNoSaldo ? AjustarSaldo(efeito.BancoId!.Value, efeito.NoSaldo, userId) : Task.CompletedTask;

    private Task Desfazer(Efeito efeito, int userId) =>
        efeito.MexeNoSaldo ? AjustarSaldo(efeito.BancoId!.Value, -efeito.NoSaldo, userId) : Task.CompletedTask;

    private async Task AjustarSaldo(int bancoId, decimal diferenca, int userId)
    {
        var banco = await bancoRepository.ObterPorIdAsync(bancoId, userId)
            ?? throw new NaoEncontradoException("Banco não encontrado.");

        await bancoRepository.UpdateSaldo(bancoId, banco.SaldoInicial + diferenca, userId);
    }
}
