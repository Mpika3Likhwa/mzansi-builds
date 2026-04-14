using System.ComponentModel.DataAnnotations;

namespace mzansi_builds_api.DTOs.ProjectStage
{
    public class CreateStageDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? SupportRequired { get; set; } = string.Empty;
    }
}
