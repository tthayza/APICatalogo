using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class PostProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;

        public PostProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task PostProduto_Return_CreatedStatusCode()
        {
            //arrange
            var novoProdutoDto = new ProdutoDTO
            {
                Nome = "Novo Produto",
                Descricao = "Descrição",
                Preco = 10.99m,
                ImagemUrl = "imagemaqui1.jpg",
                CategoriaId = 2
            };

            //act 
            var data = await _controller.Post(novoProdutoDto);

            //assert
            var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
            createdResult.Subject.StatusCode.Should().Be(201);

        }

        [Fact]
        public async Task PostProduto_Return_BadRequest()
        {
            ProdutoDTO prod = null;

            //act 
            var data = await _controller.Post(prod);

            //assert
            var badRequestResult = data.Result.Should().BeOfType<BadRequestResult>();
            badRequestResult.Subject.StatusCode.Should().Be(400);
        }

    }
}
