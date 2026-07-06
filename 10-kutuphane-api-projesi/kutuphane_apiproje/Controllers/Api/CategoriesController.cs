using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;

namespace kutuphane_apiproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public CategoriesController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetCategories")]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await dbcontext.Categories.ToListAsync();
        }

        [HttpGet]
        [Route("GetCategoryById/{id}")]
        public async Task<Category> GetCategoryById(int id)
        {
            return await dbcontext.FindAsync<Category>(id);
        }

        [HttpPost]
        [Route("AddCategory")]
        public async Task<Category> AddCategory(Category category)
        {
            dbcontext.Add(category);
            await dbcontext.SaveChangesAsync();
            return category;
        }

        [HttpPut]
        [Route("UpdateCategory/{id}")]
        public async Task<Category> UpdateCategory(Category category)
        {
            dbcontext.Update(category);
            await dbcontext.SaveChangesAsync();
            return category;
        }

        [HttpDelete]
        [Route("DeleteCategory/{id}")]
        public bool DeleteCategory(int id)
        {
            bool islem = false;
            var result = dbcontext.Categories.Find(id);

            if (result != null)
            {
                islem = true;
                dbcontext.Remove(result);
                dbcontext.SaveChanges();
            }

            return islem;
        }
    }
}