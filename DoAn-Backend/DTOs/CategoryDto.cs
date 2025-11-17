using System.ComponentModel.DataAnnotations;

namespace DoAn_Backend.DTOs
{
	

	public class CategoryDto
	{
		public int CategoryID { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public string? Description { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public int ProductCount { get; set; }
	}
}