using APIs_Trainee.dtos;
using APIs_Trainee.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Trainee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() 
        {
           var genres = await _context.Genres.OrderBy(g=>g.Name).ToListAsync();

            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody]CreateGenredto dto) 
        {
            var genre = new Genre { Name = dto.Name };
            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody]CreateGenredto dto ) 
        {
            var gen = await _context.Genres.SingleOrDefaultAsync(g=>g.Id==id);
            
            if (gen ==null)
                return NotFound($"No genre was found with id:{id}");

            gen.Name = dto.Name;
            _context.SaveChanges();
            return Ok(gen);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id) 
        {
            var gen = await _context.Genres.SingleOrDefaultAsync(g=>g.Id==id);

            if (gen == null)
                return NotFound($"No genre was found with id:{id}");

            _context.Genres.Remove(gen);
            _context.SaveChanges();

            return Ok(gen);
        }
    }
}
