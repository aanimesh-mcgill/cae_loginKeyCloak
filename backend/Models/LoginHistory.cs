using System.ComponentModel.DataAnnotations;

namespace LoginSystem.API.Models
{
    public class LoginHistory
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(256)]
        public string Username { get; set; } = string.Empty;
        
        public DateTime LoginTimeUtc { get; set; } = DateTime.UtcNow;
        
        public bool Success { get; set; }
        
        [MaxLength(45)]
        public string? IpAddress { get; set; }
        
        [MaxLength(500)]
        public string? UserAgent { get; set; }
        
        [MaxLength(500)]
        public string? FailureReason { get; set; }
        
        // Foreign key
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
    }
} 