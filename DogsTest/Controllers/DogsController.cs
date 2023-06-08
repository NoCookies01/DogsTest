using DogsTest.Contexts;
using DogsTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace DogsTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DogsController : ControllerBase
    {
        private readonly DogContext _context;

        public DogsController(DogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? attribute, string? order, int? page, int? pageSize)
        {
            if (attribute != null && !IsValidAttribute(attribute))
            {
                return BadRequest("Invalid attribute.");
            }

            List<Dog> dogs = await _context.GetAllAsync(attribute, order, page, pageSize);

            return Ok(dogs);
        }

        private bool IsValidAttribute(string attribute)
        {
            // Define the valid attributes for sorting
            string[] validAttributes = { "name", "color", "tail_length", "weight" };

            return validAttributes.Contains(attribute.ToLower());
        }

        [HttpPost]
        public async Task<IActionResult> AddDog(Dog dog)
        {
            await _context.AddDogAsync(dog);
            return Ok();
        }
    }
}

