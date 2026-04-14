using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mzansi_builds_api.Models
{
    public enum CollabStatus { Pending, Accepted, Declined, Withdrawn }

    public class CollaborationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string RequesterUserId { get; set; } = string.Empty; // Standardized name

        [MaxLength(1000)] // Increased to match DTO
        public string? Message { get; set; }

        public CollabStatus Status { get; set; } = CollabStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Standardized with DTO

        // Navigation Properties
        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }
    }
}