using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders.Testing;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers
{
    //[EnableCors("SourcesAllowedAccess")]
    [Route("[controller]")]
    [ApiController]
    [EnableRateLimiting("fixedwindow")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;

        

        public CategoriasController(IUnitOfWork uof, ILogger<CategoriasController> logger)
        {
            _uof = uof;
            _logger = logger;
        }


        [HttpGet]
        [DisableRateLimiting]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync()
        {
            var categorias = await _uof.CategoriaRepository.GetAllAsync();
            if (categorias is null)
            {
                return NotFound("Não existem categorias.");
            }

            //var categoriasDto = new List<CategoriaDTO>();
            //foreach (var categoria in categorias)
            //{
            //    var categoriaDto = new CategoriaDTO()
            //    {
            //        CategoriaId = categoria.CategoriaId,
            //        Nome = categoria.Nome,
            //        ImagemUrl = categoria.ImagemUrl,

            //    };
            //    categoriasDto.Add(categoriaDto);
            //}
            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

            return ObterCategorias(categorias);
        }

        [HttpGet("filter/nome/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas(
            [FromQuery] CategoriasFiltroNome categoriasFiltro)
        {
            var categoriasFiltradas = await _uof.CategoriaRepository.
                GetCategoriasFiltroNomeAsync(categoriasFiltro);
            return ObterCategorias(categoriasFiltradas);
        }

        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.Count,
                categorias.PageSize,
                categorias.PageCount,
                categorias.TotalItemCount,
                categorias.HasNextPage,
                categorias.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }


        [DisableCors]
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> GetAsync(int id)
        {
                var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
                
                if (categoria is null)
                {
                    _logger.LogWarning($"Categoria com id {id} não encontrada.");
                    return NotFound($"Categoria de id {id} não encontrada");
                }


            //var categoriaDto = new CategoriaDTO()
            //{
            //    CategoriaId = categoria.CategoriaId,
            //    Nome = categoria.Nome,
            //    ImagemUrl = categoria.ImagemUrl,

            //};
            var categoriaDto = categoria.ToCategoriaDTO();

                return Ok(categoriaDto);
            
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {

            if (categoriaDto == null)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            //var categoria = new Categoria()
            //{
            //    CategoriaId = categoriaDto.CategoriaId,
            //    Nome = categoriaDto.Nome,
            //    ImagemUrl= categoriaDto.ImagemUrl,
            //};
            var categoria = categoriaDto.ToCategoria(); 
             

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            await _uof.CommitAsync();
            //var novaCategoriaDto = new CategoriaDTO()
            //{
            //    CategoriaId = categoria.CategoriaId,
            //    Nome = categoria.Nome,
            //    ImagemUrl = categoria.ImagemUrl,

            //};
            var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();
            return new CreatedAtRouteResult("ObterCategoria",
                new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            //var categoria = new Categoria()
            //{
            //    CategoriaId = categoriaDto.CategoriaId,
            //    Nome = categoriaDto.Nome,
            //    ImagemUrl = categoriaDto.ImagemUrl,
            //};

            var categoria = categoriaDto.ToCategoria();

            _uof.CategoriaRepository.Update(categoria);
            await _uof.CommitAsync();

            //var categoriaDtoAtualizada = new CategoriaDTO()
            //{
            //    CategoriaId = categoria.CategoriaId,
            //    Nome = categoria.Nome,
            //    ImagemUrl = categoria.ImagemUrl,

            //};

            var categoriaDtoAtualizada = categoria.ToCategoriaDTO();
            return Ok(categoriaDtoAtualizada);
        }

        
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria == null)
            {
                _logger.LogWarning($"Categoria com id {id} não encontrada.");
                return NotFound($"Categoria com id {id} não encontrada.");
            }

            _uof.CategoriaRepository.Delete(categoria);
            await _uof.CommitAsync();

            //var categoriaDto = new CategoriaDTO()
            //{
            //    CategoriaId = categoria.CategoriaId,
            //    Nome = categoria.Nome,
            //    ImagemUrl = categoria.ImagemUrl,

            //};
            var categoriaDto = categoria.ToCategoriaDTO();
            return Ok(categoriaDto);
        }
    }
}
