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
    public class DeleteProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {

        private readonly ProdutosController _controller;
        public DeleteProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task DeleteProduto_Return_OkResult()
        {
            var prodId = 5;

            //act 
            var result = await _controller.Delete(prodId);

            //assert 
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteProduto_Return_NotFound()
        {
            //arrange
            var prodId = 555;

            //act 
            var result = await _controller.Delete(prodId) as ActionResult<ProdutoDTO>;

            //assert 
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
