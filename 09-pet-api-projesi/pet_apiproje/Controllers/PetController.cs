using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_apiproje.Models;

namespace pet_apiproje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public PetController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("GetPet")]
        public async Task<IEnumerable<Pet>> GetPet()
        {
            return await context.Pets.ToListAsync();
        }

        [HttpPost]
        [Route("AddPet")]
        public async Task<Pet> AddPet(Pet pet)
        {
            context.Pets.Add(pet);
            await context.SaveChangesAsync();

            return pet;
        }

        [HttpPut]
        [Route("UpdatePet/{id}")]
        public async Task<Pet> UpdatePet(Pet pet, int id)
        {
            pet.Id = id;

            context.Entry(pet).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return pet;
        }

        [HttpDelete]
        [Route("DeletePet/{id}")]
        public bool DeletePet(int id)
        {
            bool a = false;

            var pet = context.Pets.Find(id);

            if (pet != null)
            {
                a = true;
                context.Entry(pet).State = EntityState.Deleted;
                context.SaveChanges();
            }

            return a;
        }
    }
}