﻿using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders.Testing;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repository;
        private readonly ILogger _logger;

        

        public CategoriasController(ICategoriaRepository repository, ILogger<CategoriasController> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            var categorias = _repository.GetCategorias();
            return Ok(categorias);
        }


        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
                var categoria = _repository.GetCategoria(id);
                
                if (categoria is null)
                {
                    _logger.LogWarning($"Categoria com id {id} não encontrada.");
                    return NotFound($"Categoria de id {id} não encontrada");
                }
                return Ok(categoria);
            
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {

            if (categoria == null)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }
    
            _repository.Create(categoria);
            // _repository.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            _repository.Update(categoria);
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Categoria> Delete(int id)
        {
            var categoria = _repository.GetCategoria(id);

            if (categoria == null)
            {
                _logger.LogWarning($"Categoria com id {id} não encontrada.");
                return NotFound($"Categoria com id {id} não encontrada.");
            }

            _repository.Delete(id);
            return Ok(categoria);
        }
    }
}
