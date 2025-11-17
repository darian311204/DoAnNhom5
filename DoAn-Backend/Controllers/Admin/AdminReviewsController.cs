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
        private readonly ILogger<AdminReviewController> _logger;

        public AdminReviewController(IReviewService reviewService, ILogger<AdminReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            try
            {
                _logger.LogInformation("Admin getting all reviews");
                var reviews = await _reviewService.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{reviewId}")]
        public async Task<ActionResult<Review>> GetReview(int reviewId)
        {
            try
            {
                _logger.LogInformation("Admin getting review {ReviewId}", reviewId);
                var review = await _reviewService.GetReviewByIdAsync(reviewId);

                if (review == null)
                {
                    _logger.LogWarning("Review {ReviewId} not found", reviewId);
                    return NotFound(new { message = "Review not found" });
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review {ReviewId}", reviewId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{reviewId}/toggle-active")]
        public async Task<IActionResult> ToggleReviewActive(int reviewId)
        {
            try
            {
                _logger.LogInformation("Admin toggling active status for review {ReviewId}", reviewId);
                var success = await _reviewService.ToggleReviewActiveAsync(reviewId);

                if (!success)
                {
                    _logger.LogWarning("Failed to toggle review {ReviewId} - not found", reviewId);
                    return NotFound(new { success = false, message = "Review not found" });
                }

                _logger.LogInformation("Successfully toggled review {ReviewId} status", reviewId);
                return Ok(new { success = true, message = "Review status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling review {ReviewId}", reviewId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                _logger.LogInformation("Admin deleting review {ReviewId}", reviewId);
                var success = await _reviewService.DeleteReviewAsync(reviewId);

                if (!success)
                {
                    _logger.LogWarning("Failed to delete review {ReviewId} - not found", reviewId);
                    return NotFound(new { success = false, message = "Review not found" });
                }

                _logger.LogInformation("Successfully deleted review {ReviewId}", reviewId);
                return Ok(new { success = true, message = "Review deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetProductReviews(int productId)
        {
            try
            {
                _logger.LogInformation("Admin getting reviews for product {ProductId}", productId);
                var reviews = await _reviewService.GetProductReviewsAsync(productId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("statistics/{productId}")]
        public async Task<ActionResult<ReviewStatistics>> GetReviewStatistics(int productId)
        {
            try
            {
                _logger.LogInformation("Admin getting review statistics for product {ProductId}", productId);
                var stats = await _reviewService.GetReviewStatisticsAsync(productId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review statistics for product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("statistics/overall")]
        public async Task<ActionResult> GetOverallStatistics()
        {
            try
            {
                _logger.LogInformation("Admin getting overall review statistics");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overall statistics");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}