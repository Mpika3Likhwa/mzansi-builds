namespace mzansi_builds_api.DTOs;

public class ProjectCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<StageCreateDto> Stages { get; set; } = new();
}

public class StageCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string SupportRequired { get; set; } = string.Empty;
}