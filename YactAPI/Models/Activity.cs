using System.ComponentModel.DataAnnotations;

namespace YactAPI.Models
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Path { get; set; }

        // Useful but not required fields
        public string? ContentType { get; set; }
        public long? FileSize { get; set; }
        public DateTime? UploadDate { get; set; }
    }
}