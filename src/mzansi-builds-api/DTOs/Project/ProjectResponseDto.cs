using mzansi_builds_api.DTOs.ProjectStage;

namespace mzansi_builds_api.DTOs.Project
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Audit & Ownership
        public string DeveloperId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Status for the Celebration Wall
        public bool IsFullyCompleted { get; set; }

        // The Roadmap
        public List<StageResponseDto> Stages { get; set; } = new();
    }
}
