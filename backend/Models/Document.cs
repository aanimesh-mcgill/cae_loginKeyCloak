using System.ComponentModel.DataAnnotations;

namespace LoginSystem.API.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(256)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(256)]
        public string? UpdatedBy { get; set; }
    }
} 