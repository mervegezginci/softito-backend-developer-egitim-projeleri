using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;

namespace kutuphane_apiproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingsController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public BorrowingsController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetBorrowings")]
        public async Task<IEnumerable<Borrowing>> GetBorrowings()
        {
            return await dbcontext.Borrowings
                .Include(x => x.Book)
                .Include(x => x.User)
                .ToListAsync();
        }

        [HttpGet]
        [Route("GetBorrowingById/{id}")]
        public async Task<Borrowing> GetBorrowingById(int id)
        {
            return await dbcontext.FindAsync<Borrowing>(id);
        }

        [HttpPost]
        [Route("AddBorrowing")]
        public async Task<Borrowing> AddBorrowing(Borrowing borrowing)
        {
            dbcontext.Add(borrowing);
            await dbcontext.SaveChangesAsync();
            return borrowing;
        }

        [HttpPut]
        [Route("UpdateBorrowing/{id}")]
        public async Task<Borrowing> UpdateBorrowing(Borrowing borrowing)
        {
            dbcontext.Update(borrowing);
            await dbcontext.SaveChangesAsync();
            return borrowing;
        }

        [HttpDelete]
        [Route("DeleteBorrowing/{id}")]
        public bool DeleteBorrowing(int id)
        {
            bool islem = false;
            var result = dbcontext.Borrowings.Find(id);

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