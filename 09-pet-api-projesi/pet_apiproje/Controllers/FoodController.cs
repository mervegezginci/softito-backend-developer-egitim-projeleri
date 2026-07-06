using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_apiproje.Models;

namespace pet_apiproje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public FoodController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("GetFood")]
        public async Task<IEnumerable<Food>> GetFood()
        {
            return await context.Foods.ToListAsync();
        }

        [HttpPost]
        [Route("AddFood")]
        public async Task<Food> AddFood(Food food)
        {
            context.Foods.Add(food);
            await context.SaveChangesAsync();

            return food;
        }

        [HttpPut]
        [Route("UpdateFood/{id}")]
        public async Task<Food> UpdateFood(Food food, int id)
        {
            food.Id = id;

            context.Entry(food).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return food;
        }

        [HttpDelete]
        [Route("DeleteFood/{id}")]
        public bool DeleteFood(int id)
        {
            bool a = false;

            var food = context.Foods.Find(id);

            if (food != null)
            {
                a = true;
                context.Entry(food).State = EntityState.Deleted;
                context.SaveChanges();
            }

            return a;
        }
    }
}