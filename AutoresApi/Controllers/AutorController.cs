using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoresApi;
using AutoresApi.Models;
using AutoresApi.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoresApi.Utilities;

namespace AutoresApi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Es admin")]
    [Route("api/[controller]")]
    public class AutorController : ControllerBase
    {
        private readonly AplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        public AutorController(
            AplicationDbContext context, 
            IMapper mapper, 
            IConfiguration configuration,
            IAuthorizationService authorizationService
        )
        {
            _context = context;
            _mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }

        // GET: api/Autor
        [HttpGet("getAuthors", Name = "getAuthors")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> GetAutores([FromHeader] string? incluirHATEOAS)
        {
          if (_context.Autores == null)
          {
              return NotFound();
          }
            var  autores =  await _context.Autores.ToListAsync();
            var result = _mapper.Map<List<AutorDTO>>(autores);
            return Ok(result);
        }

        // GET: api/Autor/5
        [HttpGet("getAuthorById/{id}", Name = "getAuthorById")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> GetAutor(int id, [FromHeader] string? incluirHATEOAS)
        {
            if (_context.Autores == null)
            {
                return NotFound();
            }
            var autor = await _context.Autores.Include(x => x.AutoresLibros).ThenInclude(x => x.Libro).FirstOrDefaultAsync(x=> x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto =  _mapper.Map<AutorDTOConLibros>(autor);
            return dto;
        }

        [HttpGet("getAuthorByName/{nombre}", Name = "getAuthorByName")]
        public async Task<ActionResult<List<AutorDTO>>> GetAutor([FromRoute] string nombre)
        {
            var autores = await _context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            if (autores == null)
            {
                return NotFound();
            }

            return _mapper.Map<List<AutorDTO>>(autores);
        }

        // PUT: api/Autor/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("updateAuthor/{id}", Name = "updateAuthor")]
        public async Task<IActionResult> PutAutor(int id, CrearAutorDTO autorDTO)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }
            var autor = _mapper.Map<Autor>(autorDTO);
            autor.Id = id;
            try
            {
                _context.Update(autor); 
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Autor
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("createAuthor", Name = "createAuthor")]
        public async Task<ActionResult> PostAutor([FromBody] CrearAutorDTO crearAutorDTO)
        {
            var validateExist = await _context.Autores.AnyAsync(x => x.Nombre == crearAutorDTO.Nombre);
            
            if (validateExist) return BadRequest($"Ya existe un autor con el nombre {crearAutorDTO.Nombre}");
            //Cast
            var autor = _mapper.Map<Autor>(crearAutorDTO);
            _context.Add(autor);
             await _context.SaveChangesAsync();
            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("getAuthorById", new { id = autor.Id}, autorDTO);
        }

        // DELETE: api/Autor/5
        [HttpDelete("deleteAuthor/{id}", Name = "deleteAuthor")]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }
            _context.Remove(new Autor() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private bool AutorExists(int id)
        {
            return (_context.Autores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
