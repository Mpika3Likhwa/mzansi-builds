namespace mzansi_builds_api.DTOs.ProjectStage
{
    public class StageResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SupportRequired { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
