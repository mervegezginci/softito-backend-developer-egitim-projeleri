using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;

namespace kutuphane_apiproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public BooksController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetBooks")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await dbcontext.Books
                .Include(x => x.Author)
                .Include(x => x.Category)
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet]
        [Route("GetBookById/{id}")]
        public async Task<Book> GetBookById(int id)
        {
            return await dbcontext.FindAsync<Book>(id);
        }

        [HttpPost]
        [Route("AddBook")]
        public async Task<Book> AddBook(Book book)
        {
            dbcontext.Add(book);
            await dbcontext.SaveChangesAsync();
            return book;
        }

        [HttpPut]
        [Route("UpdateBook/{id}")]
        public async Task<Book> UpdateBook(Book book)
        {
            dbcontext.Update(book);
            await dbcontext.SaveChangesAsync();
            return book;
        }

        [HttpDelete]
        [Route("DeleteBook/{id}")]
        public bool DeleteBook(int id)
        {
            bool islem = false;
            var result = dbcontext.Books.Find(id);

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