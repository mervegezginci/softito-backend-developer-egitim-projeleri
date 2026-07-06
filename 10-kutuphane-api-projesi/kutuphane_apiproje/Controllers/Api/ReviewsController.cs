using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;

namespace kutuphane_apiproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;

        public ReviewsController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetReviews")]
        public async Task<IEnumerable<Review>> GetReviews()
        {
            return await dbcontext.Reviews
                .Include(x => x.Book)
                .Include(x => x.User)
                .ToListAsync();
        }

        [HttpGet]
        [Route("GetReviewById/{id}")]
        public async Task<Review> GetReviewById(int id)
        {
            return await dbcontext.FindAsync<Review>(id);
        }

        [HttpPost]
        [Route("AddReview")]
        public async Task<Review> AddReview(Review review)
        {
            review.CreatedDate = DateTime.Now;

            dbcontext.Add(review);
            await dbcontext.SaveChangesAsync();
            return review;
        }

        [HttpPut]
        [Route("UpdateReview/{id}")]
        public async Task<Review> UpdateReview(Review review)
        {
            dbcontext.Update(review);
            await dbcontext.SaveChangesAsync();
            return review;
        }

        [HttpDelete]
        [Route("DeleteReview/{id}")]
        public bool DeleteReview(int id)
        {
            bool islem = false;
            var result = dbcontext.Reviews.Find(id);

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