using System.ComponentModel.DataAnnotations;

namespace mzansi_builds_api.DTOs.Collaboration
{
    public class UpdateCollaborationRequestStatusDto
    {
        [Required]
        [RegularExpression("Pending|Accepted|Declined", ErrorMessage = "Status must be Pending, Accepted or Declined")]
        public string Status { get; set; } = string.Empty;
    }
}
