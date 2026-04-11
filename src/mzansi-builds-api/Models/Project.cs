using System.ComponentModel.DataAnnotations;

namespace mzansi_builds_api.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ownership - linked to the logged-in Developer
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Celebration Wall status
        public bool IsFullyCompleted { get; set; } = false;

        // Relationships
        public List<ProjectStage> Stages { get; set; } = new();
    }
}