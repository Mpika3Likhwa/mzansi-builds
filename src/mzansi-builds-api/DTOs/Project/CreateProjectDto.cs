using mzansi_builds_api.DTOs.ProjectStage;

namespace mzansi_builds_api.DTOs.Project
{
    public class CreateProjectDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CreateStageDto> Stages { get; set; } = new();
    }
}
