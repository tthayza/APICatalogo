using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.Models;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class PutProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;



        public PutProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task PutProduto_Return_OkResult()
        {
            //arrange
            var prodId = 5;

        
            var updateProdutoDto = new ProdutoDTO
            {
                ProdutoId = prodId,
                Nome = "Produto Att",
                Descricao = "Desc",
                ImagemUrl = "novimg.jpg",
                CategoriaId = 2,
            };

            //act 
            var result = await _controller.Put(prodId, updateProdutoDto) as ActionResult<ProdutoDTO>;

            //assert 
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutProduto_Return_BadRequest()
        {
            //arrange
            var prodId = 665;

            var myProd = new ProdutoDTO{
                ProdutoId = 5454,
                Nome = "Produto Att",
                Descricao = "Desc",
                ImagemUrl = "novimg.jpg",
                CategoriaId = 2,
            };

            //act 
            var data = await _controller.Put(prodId,myProd);


            //assert
            data.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be(400);

        }


    }
}
