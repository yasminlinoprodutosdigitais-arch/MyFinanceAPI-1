using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class ListaService(IListaRepository listaRepository, IMapper mapper)
    : ServicoCadastro<Lista, ListaDTO>(listaRepository, mapper), IListaService
{
}
