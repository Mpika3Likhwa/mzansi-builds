using System.ComponentModel.DataAnnotations;

namespace mzansi_builds_api.DTOs.Collaboration
{
    public class CreateCollaborationRequestDto
    {
        [Required]
        public int ProjectId { get; set; }

        [StringLength(1000)]
        public string? Message { get; set; } 
    }
}