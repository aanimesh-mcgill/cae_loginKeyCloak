using System.ComponentModel.DataAnnotations;

namespace LoginSystem.API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(256)]
        public string AdUsername { get; set; } = string.Empty;
        
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(256)]
        public string DisplayName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Viewer"; // Default role
        
        public DateTime LastLogin { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();
    }
} 