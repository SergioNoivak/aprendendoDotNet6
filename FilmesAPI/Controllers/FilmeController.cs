using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.DTos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]  
    public IActionResult AdicionaFilme([FromBody]CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Add(filme);
        _context.SaveChanges();
            return CreatedAtAction(nameof(recuperaFilmePorId)
                ,new {id = filme.id}
                ,filme);
    }

    [HttpGet]
    public IEnumerable<ReadFilmeDto> RecuperaFilme([FromQuery]int skip=0, [FromQuery] int take=50)
    {
        return _mapper.Map<List<ReadFilmeDto>>( _context.Filmes.Skip(skip).Take(take));
 }
    [HttpGet("{id}")]
    public IActionResult? recuperaFilmePorId(int id)
    {
       var filme = _context.Filmes.FirstOrDefault((Filme filme)=> filme.id==id );
        if (filme == null)
        {
            return NotFound();
        }
        else
        {
            var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
            return Ok(filmeDto);
        }
    }
    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.id == id);
        if (filme == null)
        {
            return NotFound();
        }
        else
        {
            _mapper.Map(filmeDto, filme);
            _context.SaveChanges();
            return NoContent();
        }

    }
    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.id == id);
        if (filme == null)
        {
            return NotFound();
        }
        else
        {
            var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);
            patch.ApplyTo(filmeParaAtualizar, ModelState);
            if (!TryValidateModel(filmeParaAtualizar))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(filmeParaAtualizar, filme);
            _context.SaveChanges();
            return NoContent();
        }
    }
    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.id == id);
        if (filme == null)
        {
            return NotFound();
        }
        else
        {
            _context.Remove(filme);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
