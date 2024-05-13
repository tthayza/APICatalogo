using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class ProdutosUnitTestController
    {

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddUserSecrets<ProdutosUnitTestController>();

        IConfigurationRoot configuration = builder.Build();
        

        string connectionString = configuration.GetConnectionString("DefaultConnection");
        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public IUnitOfWork repository;
        public IMapper mapper;



        static ProdutosUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;
        }

        public ProdutosUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProdutoDTOMappingProfile());
            });

            mapper = config.CreateMapper();

            var context = new AppDbContext(dbContextOptions);
            repository = new UnitOfWork(context);
        }

    }
}
