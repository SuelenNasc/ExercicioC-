using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using loja.models;
using loja.data;

namespace Loja.data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<LojaDbContext>
    {
        public LojaDbContext CreateDbContext(String[] args){
            var optionsBuilder = new DbContextOptionsBuilder<LojaDbContext>();

            //build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8,0,36)));

            return new LojaDbContext(optionsBuilder.Options);
        }
    }
}