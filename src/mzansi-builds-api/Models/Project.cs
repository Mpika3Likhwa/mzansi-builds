using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mzansi_builds_api.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ownership - linked to the logged-in Developer
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Celebration Wall status
        public bool IsFullyCompleted { get; set; } = false;

        // GitHub integration
        [Url]
        public string GitHubRepoUrl { get; set; } = string.Empty;

        public bool IsGitHubRepoPrivate { get; set; } = false;

        // Relationships
        public List<ProjectStage> Stages { get; set; } = new();
    }
}