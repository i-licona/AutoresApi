using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoresApi.Models;
using AutoresApi.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace AutoresApi.Controllers.V1
{
    [Route("api/v1/libro")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly AplicationDbContext _context;
        private readonly IMapper _mapper;

        public LibroController(AplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Libro
        [HttpGet("getBooks", Name = "getBooks")]
        public async Task<ActionResult<IEnumerable<LibroDTO>>> GetLibros()
        {
            if (_context.Libros == null)
            {
                return NotFound();
            }
            var libros = await _context.Libros.Include(libro => libro.Comentarios).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<LibroDTO>>(libros));
        }

        // GET: api/Libro/5
        [HttpGet("getBookById/{id}", Name = "getBookById")]
        public async Task<ActionResult<LibroDTOConAutores>> GetLibro(int id)
        {
            if (_context.Libros == null)
            {
                return NotFound();
            }
            var libro = await _context.Libros.Include(x => x.AutoresLibros).ThenInclude(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return _mapper.Map<LibroDTOConAutores>(libro);
        }

        // PUT: api/Libro/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("updateBook/{id}", Name = "updateBook")]
        public async Task<IActionResult> PutLibro(int id, LibroCrearDTO libroDTO)
        {
            var libroDB = await _context.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(x => x.Id == id);
            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = _mapper.Map(libroDTO, libroDB);
            AsignarOrdenAutores(libroDB);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Libro
        [HttpPost("createBook", Name = "createBook")]
        public async Task<ActionResult<LibroCrearDTO>> PostLibro(LibroCrearDTO libroCrearDTO)
        {
            if (libroCrearDTO.AutoresIds == null)
            {
                return BadRequest("No es posible crear un libro sin autores");
            }

            var autoresIds = await _context.Autores.Where(x => libroCrearDTO.AutoresIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            if (libroCrearDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = _mapper.Map<Libro>(libroCrearDTO);
            /* Designar orden de autores */
            AsignarOrdenAutores(libro);
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            var libroDTO = _mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("getBookById", new { id = libro.Id }, libroDTO);
        }

        // DELETE: api/Libro/5
        [HttpDelete("deleteBook/{id}", Name = "deleteBook")]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            var existe = await _context.Libros.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }
            _context.Remove(new Libro() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool LibroExists(int id)
        {
            return (_context.Libros?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPatch("patchBook/{id}", Name = "patchBook")]
        public async Task<ActionResult> PatchLibro(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var libroDb = await _context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (libroDb == null)
            {
                return NotFound();
            }
            var libroDTO = _mapper.Map<LibroPatchDTO>(libroDb);
            patchDocument.ApplyTo(libroDTO, ModelState);
            var isValid = TryValidateModel(libroDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(libroDTO, libroDb);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            /* Designar orden de autores */
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }
}
