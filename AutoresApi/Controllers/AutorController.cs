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

        public AutorController(AplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            this.configuration = configuration;
        }

        // GET: api/Autor
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> GetAutores()
        {
          if (_context.Autores == null)
          {
              return NotFound();
          }
            var autores =  await _context.Autores.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<AutorDTO>>(autores));
        }

        // GET: api/Autor/5
        [HttpGet("{id}",Name ="getAutorById")]
        public async Task<ActionResult<AutorDTOConLibros>> GetAutor(int id)
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

            return _mapper.Map<AutorDTOConLibros>(autor);
        }
        [HttpGet("buscar-por-nombre/{nombre}")]
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
        [HttpPut("{id}")]
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
        [HttpPost]
        public async Task<ActionResult> PostAutor([FromBody] CrearAutorDTO crearAutorDTO)
        {
            var validateExist = await _context.Autores.AnyAsync(x => x.Nombre == crearAutorDTO.Nombre);
            
            if (validateExist) return BadRequest($"Ya existe un autor con el nombre {crearAutorDTO.Nombre}");
            //Cast
            var autor = _mapper.Map<Autor>(crearAutorDTO);
            _context.Add(autor);
             await _context.SaveChangesAsync();
            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("getAutorById", new { id = autor.Id}, autorDTO);
        }

        // DELETE: api/Autor/5
        [HttpDelete("{id}")]
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
