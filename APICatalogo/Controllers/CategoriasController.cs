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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders.Testing;
using Newtonsoft.Json;
using X.PagedList;
using static System.Net.Mime.MediaTypeNames;

namespace APICatalogo.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[EnableCors("SourcesAllowedAccess")]
    [Route("[controller]")]
    [ApiController]
    [EnableRateLimiting("fixedwindow")]
    [Produces("application/json")]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;

        

        public CategoriasController(IUnitOfWork uof, ILogger<CategoriasController> logger)
        {
            _uof = uof;
            _logger = logger;
        }

        /// <summary>
        /// Obtem uma lista de objetos Categoria
        /// </summary>
        /// <returns>Uma lista de objetos Categoria</returns>

        [HttpGet]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
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

        /// <summary>
        /// Obtem uma categoria pelo seu id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objeto Categoria</returns>
        [DisableCors]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        ///    Exemplo de request:
        ///    POST api/categorias
        ///    {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "http://teste.net/1.jpg"
        ///    }
        /// </remarks>
        /// <param name="categoriaDto">objeto Categoria</param>
        /// <returns>O Objeto Categoria inclusa</returns>
        /// <remarks>Retorna um objeto Categoria incluído</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
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

            _ = _uof.CategoriaRepository.Update(categoria);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
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
