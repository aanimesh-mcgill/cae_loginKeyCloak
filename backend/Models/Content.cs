using System.ComponentModel.DataAnnotations;

namespace LoginSystem.API.Models
{
    public class Content
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        [MaxLength(256)]
        public string CreatedBy { get; set; } = string.Empty;
        
        [MaxLength(256)]
        public string? UpdatedBy { get; set; }
        
        public bool IsPublished { get; set; } = false;
    }
} 