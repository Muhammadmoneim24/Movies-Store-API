using APIs_Trainee.dtos;
using APIs_Trainee.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Trainee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private new List<string> _allowedExtensions = new List<String>() { ".jpg",".png"};
        private new long _maxAllowedPosterSize =1048576 ;


        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() 
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Rate).Include(m=>m.Genre).ToListAsync();

            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound("Movie is not found");

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]CreateMoviedto dto) 
        {
            if (dto.Poster == null)
                return BadRequest("Poster is required");

            if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName.ToLower())))
                return BadRequest("Only an pnj and jpg are allowed");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max Allowed Size is 1MB");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID");

            using var dataStraem = new MemoryStream();

            await dto.Poster.CopyToAsync(dataStraem);

            var movie = new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Poster = dataStraem.ToArray(),
                GenreId = dto.GenreId,
            };
            await _context.Movies.AddAsync(movie);
            _context.SaveChanges();

            return Ok(movie);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromForm]CreateMoviedto dto) 
        {

           var movie = await _context.Movies.FirstOrDefaultAsync(g => g.Id == id);

            if (movie == null)
                return NotFound("Movie is not found");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID");

            if(dto.Poster != null) 
            {
                if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName.ToLower())))
                    return BadRequest("Only an pnj and jpg are allowed");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max Allowed Size is 1MB");

                using var dataStraem = new MemoryStream();

                await dto.Poster.CopyToAsync(dataStraem);

                movie.Poster = dataStraem.ToArray();
            }

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.StoryLine = dto.StoryLine;
            movie.GenreId = dto.GenreId;


            _context.SaveChanges();

            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id) 
        {
            if (!await _context.Movies.AnyAsync(m => m.Id == id))
                return NotFound("The Id is not found");

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound("Movie is not found");

            _context.Remove(movie);
            _context.SaveChanges(true);

            return Ok(movie);

        }
    }
}
