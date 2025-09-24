using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MyFinanceAPI.Data.Context;

public class ContextDBFactory : IDesignTimeDbContextFactory<ContextDB>
{
    public ContextDB CreateDbContext(string[] args)
    {
        // 1) Permite usar env var: ConnectionStrings__DefaultConnection
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrEmpty(cs))
        {
            // 2) Resolve caminho da API de forma relativa ao bin do projeto Data
            var baseDir  = AppContext.BaseDirectory;                         // ...\MyFinanceAPI.Data\bin\Debug\net8.0\
            var dataProj = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var apiProj  = Path.GetFullPath(Path.Combine(dataProj, "..", "MyFinanceAPI.Api"));

            var cfg = new ConfigurationBuilder()
                .SetBasePath(apiProj)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            cs = cfg.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' não encontrada. " +
                    "Defina no appsettings da API ou na env 'ConnectionStrings__DefaultConnection'.");
        }

        var options = new DbContextOptionsBuilder<ContextDB>()
            .UseNpgsql(cs)
            .Options;

        return new ContextDB(options);
    }
}
