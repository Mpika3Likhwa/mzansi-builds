src\mzansi-builds-api\DTOs\Collaboration\CreateCollaborationRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace mzansi_builds_api.DTOs.Collaboration
{
    public class CreateCollaborationRequestDto
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}   