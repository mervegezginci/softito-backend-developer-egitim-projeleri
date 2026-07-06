using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_apiproje.Models;

namespace pet_apiproje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public OwnerController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("GetOwner")]
        public async Task<IEnumerable<Owner>> GetOwner()
        {
            return await context.Owners.ToListAsync();
        }

        [HttpPost]
        [Route("AddOwner")]
        public async Task<Owner> AddOwner(Owner owner)
        {
            context.Owners.Add(owner);
            await context.SaveChangesAsync();

            return owner;
        }

        [HttpPut]
        [Route("UpdateOwner/{id}")]
        public async Task<Owner> UpdateOwner(Owner owner, int id)
        {
            owner.Id = id;

            context.Entry(owner).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return owner;
        }

        [HttpDelete]
        [Route("DeleteOwner/{id}")]
        public bool DeleteOwner(int id)
        {
            bool a = false;

            var owner = context.Owners.Find(id);

            if (owner != null)
            {
                a = true;
                context.Entry(owner).State = EntityState.Deleted;
                context.SaveChanges();
            }

            return a;
        }
    }
}