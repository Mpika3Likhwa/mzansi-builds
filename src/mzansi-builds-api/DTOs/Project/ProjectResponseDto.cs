using System;
using System.Collections.Generic;
using mzansi_builds_api.DTOs.ProjectStage;

namespace mzansi_builds_api.DTOs.Project
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string DeveloperId { get; set; } = string.Empty;
        public string DeveloperName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public bool IsFullyCompleted { get; set; }
        public string GitHubRepoUrl { get; set; } = string.Empty;
        public bool IsGitHubRepoPrivate { get; set; }
        public List<StageResponseDto> Stages { get; set; } = new();
        public int ProgressPercentage { get; set; } = 0;
    }
}
