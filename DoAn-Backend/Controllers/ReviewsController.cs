using DoAn_Backend.Models;
using DoAn_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        // Public endpoint - không c?n authentication
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetProductReviews(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetProductReviewsAsync(productId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // Public endpoint - không c?n authentication
        [HttpGet("statistics/{productId}")]
        public async Task<ActionResult<ReviewStatistics>> GetReviewStatistics(int productId)
        {
            try
            {
                var stats = await _reviewService.GetReviewStatisticsAsync(productId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review statistics for product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // C?n authentication ð? thêm review
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Review>> AddReview([FromBody] AddReviewRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("AddReview called without valid user ID");
                    return Unauthorized(new { message = "User not authenticated" });
                }

                // Ki?m tra user ð? review s?n ph?m này chýa
                var hasReviewed = await _reviewService.UserHasReviewedProductAsync(request.ProductId, userId);
                if (hasReviewed)
                {
                    _logger.LogWarning("User {UserId} already reviewed product {ProductId}", userId, request.ProductId);
                    return BadRequest(new { message = "You have already reviewed this product" });
                }

                var review = await _reviewService.AddReviewAsync(
                    request.ProductId,
                    userId,
                    request.Rating,
                    request.Comment ?? string.Empty
                );

                _logger.LogInformation("User {UserId} added review for product {ProductId}", userId, request.ProductId);
                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        public class AddReviewRequest
        {
            public int ProductId { get; set; }
            public int Rating { get; set; }
            public string? Comment { get; set; }
        }
    }
}