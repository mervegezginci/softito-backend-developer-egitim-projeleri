using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;

namespace kutuphane_apiproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public AuthorsController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetAuthors")]
        public async Task<IEnumerable<Author>> GetAuthors()
        {
            return await dbcontext.Authors.ToListAsync();
        }

        [HttpGet]
        [Route("GetAuthorById/{id}")]
        public async Task<Author> GetAuthorById(int id)
        {
            return await dbcontext.FindAsync<Author>(id);
        }

        [HttpPost]
        [Route("AddAuthor")]
        public async Task<Author> AddAuthor(Author author)
        {
            dbcontext.Add(author);
            await dbcontext.SaveChangesAsync();
            return author;
        }

        [HttpPut]
        [Route("UpdateAuthor/{id}")]
        public async Task<Author> UpdateAuthor(Author author)
        {
            dbcontext.Update(author);
            await dbcontext.SaveChangesAsync();
            return author;
        }

        [HttpDelete]
        [Route("DeleteAuthor/{id}")]
        public bool DeleteAuthor(int id)
        {
            bool islem = false;
            var result = dbcontext.Authors.Find(id);

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