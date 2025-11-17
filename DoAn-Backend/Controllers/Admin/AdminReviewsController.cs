using DoAn_Backend.Models;
using DoAn_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Backend.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public AdminReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        public async Task<ActionResult<Review>> GetReview(int reviewId)
        {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);

            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [HttpPut("{reviewId}/toggle-active")]
        public async Task<IActionResult> ToggleReviewActive(int reviewId)
        {
            var success = await _reviewService.ToggleReviewActiveAsync(reviewId);

            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var success = await _reviewService.DeleteReviewAsync(reviewId);

            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(reviews);
        }

        [HttpGet("statistics/{productId}")]
        public async Task<ActionResult<ReviewStatistics>> GetReviewStatistics(int productId)
        {
            var stats = await _reviewService.GetReviewStatisticsAsync(productId);
            return Ok(stats);
        }

        [HttpGet("statistics/overall")]
        public async Task<ActionResult> GetOverallStatistics()
        {
            var allReviews = await _reviewService.GetAllReviewsAsync();

            var stats = new
            {
                TotalReviews = allReviews.Count(),
                ActiveReviews = allReviews.Count(r => r.IsActive),
                InactiveReviews = allReviews.Count(r => !r.IsActive),
                AverageRating = allReviews.Any() ? allReviews.Where(r => r.IsActive).Average(r => r.Rating) : 0,
                RatingDistribution = Enumerable.Range(1, 5).ToDictionary(
                    i => i,
                    i => allReviews.Count(r => r.Rating == i && r.IsActive)
                ),
                RecentReviews = allReviews.OrderByDescending(r => r.CreatedAt).Take(5)
            };

            return Ok(stats);
        }
    }
}