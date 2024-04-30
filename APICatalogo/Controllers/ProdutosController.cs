﻿using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {

        private readonly IProdutoRepository _repository;

        public ProdutosController(IProdutoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _repository.GetProdutos().ToList();
            if (produtos is null)
            {
                return NotFound();
            }
            return Ok(produtos);
        }

        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _repository.GetProduto(id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
                return BadRequest();

            _repository.Create(produto);

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }

            bool produtoAtualizado = _repository.Update(produto);
            if (produtoAtualizado)
            {
                return Ok(produto);
            } 
            
            return StatusCode(500, $"Falha ao atualizar o produto de id {id}");
            

        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produtoDeletado = _repository.Delete(id);

            if (produtoDeletado)
            {
                return Ok($"Produto com id {id} deletado com sucesso!");
            }
            return StatusCode(500, $"Falha ao deletar produto com {id}!");

        }

    }
}
