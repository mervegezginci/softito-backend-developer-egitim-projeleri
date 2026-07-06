using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_apiproje.Models;

namespace pet_apiproje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToyController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ToyController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("GetToy")]
        public async Task<IEnumerable<Toy>> GetToy()
        {
            return await context.Toys.ToListAsync();
        }

        [HttpPost]
        [Route("AddToy")]
        public async Task<Toy> AddToy(Toy toy)
        {
            context.Toys.Add(toy);
            await context.SaveChangesAsync();

            return toy;
        }

        [HttpPut]
        [Route("UpdateToy/{id}")]
        public async Task<Toy> UpdateToy(Toy toy, int id)
        {
            toy.Id = id;

            context.Entry(toy).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return toy;
        }

        [HttpDelete]
        [Route("DeleteToy/{id}")]
        public bool DeleteToy(int id)
        {
            bool a = false;

            var toy = context.Toys.Find(id);

            if (toy != null)
            {
                a = true;
                context.Entry(toy).State = EntityState.Deleted;
                context.SaveChanges();
            }

            return a;
        }
    }
}