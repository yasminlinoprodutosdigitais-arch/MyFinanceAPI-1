using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyFinanceAPI.Data.Context;
using System.IO;

namespace MyFinanceAPI.Data.Context;
public class ContextDBFactory : IDesignTimeDbContextFactory<ContextDB>
{
    public ContextDB CreateDbContext(string[] args)
    {
        // Carregar a configuração do appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(@"C:\Users\yasminl\Desktop\LINGUAGENS\PDI\MyFinanceAPI\MyFinanceAPI.Api")  // Usar o diretório atual
            .AddJsonFile("appsettings.json")  // Carregar o arquivo de configuração
            .Build();

        // Criar as opções do DbContext
        var optionsBuilder = new DbContextOptionsBuilder<ContextDB>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));  // Usar a string de conexão com PostgreSQL

        // Criar e retornar o DbContext com as opções configuradas
        return new ContextDB(optionsBuilder.Options);
    }
}
