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
    public class GetProdutoUnitTests: IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;
        public GetProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task GetProdutoById_OKResult()
        {
            //arrange
            var prodId = 2;


            //act 
            var data = await _controller.Get(prodId);

            //assert (xunit)
            //var okResult = Assert.IsType<OkObjectResult>(data.Result);
            //Assert.Equal(200, okResult.StatusCode);

            //assert (fluentassertions)
            data.Result.Should().BeOfType<OkObjectResult>() // verifica se o result é do tipo OkObjectResult
                .Which.StatusCode.Should().Be(200);         // verifica se o cód de status do OkObjectResult é 200        
        }

        [Fact]
        public async Task GetProdutoBYId_Return_NotFound()
        {
            //arrange
            var prodId = 999;

            //act 
            var data = await _controller.Get( prodId);

            //assert
            data.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetProdutoById_Return_BadRequest()
        {
            //arrange 
            int prodId = -2;

            //act 
            var data = await _controller.Get(prodId);

            //assert
            data.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetProdutos_Return_ListOfProductsDTO()
        {
            //act
            var data = await _controller.Get();

            //assert
            data.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTO>>()
                .And.NotBeNull();
        }

        [Fact]
        public async Task GetProdutos_Return_BadRequestResult()
        {
            //act
            var data = await _controller.Get();

            //assert
            data.Result.Should().BeOfType<BadRequestResult>();
        }
    }
}
