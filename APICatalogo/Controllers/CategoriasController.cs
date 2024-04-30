﻿using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
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
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
       
        public CategoriasController(AppDbContext context, ILogger<CategoriasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("produtos")] 
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutos()

        {


            _logger.LogInformation(" ========================= ====================== get/categorias/produtos");
            return await _context.Categorias.Include(p=> p.Produtos).Where(c => c.CategoriaId <=5).ToListAsync();
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get()
        {
            return await _context.Categorias.AsNoTracking().ToListAsync();   
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
                var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);
                
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
    

            _context.Categorias.Add(categoria);
            _context.SaveChanges();


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

            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Categoria> Delete(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);
            //var categoria = _context.Categorias.Find(id);

            if (categoria == null)
            {
                _logger.LogWarning($"Categoria com id {id} não encontrada.");
                return NotFound($"Categoria com id {id} não encontrada.");
            }

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return Ok(categoria);
        }
    }
}
