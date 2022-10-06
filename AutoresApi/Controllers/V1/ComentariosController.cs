using AutoMapper;
using AutoresApi.DTOs;
using AutoresApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId}/comentarios")]
    //[ApiConventionType(typeof(DefaultApiConventions))]
    public class ComentariosController : ControllerBase
    {
        public AplicationDbContext context { get; }
        public IMapper mapper { get; }

        public ComentariosController(AplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpPost("createCommit", Name = "createCommit")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCrear comentarioDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioDTO);
            comentario.LibroId = libroId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("getCommitById", new { id = comentario.Id, libroId = comentario.LibroId }, comentDTO);
        }
        [HttpGet("getCommits", Name = "getCommits")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.Where(comentario => comentario.LibroId == libroId).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }
        [HttpGet("getCommitById/{id}", Name = "getCommitById")]
        public async Task<ActionResult<ComentarioDTO>> GetById([FromRoute] int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPut("updateCommit/{id}", Name = "updateCommit")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCrear comentarioDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.AnyAsync(x => x.Id == id);

            if (!existeComentario)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            try
            {
                context.Update(comentario);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
