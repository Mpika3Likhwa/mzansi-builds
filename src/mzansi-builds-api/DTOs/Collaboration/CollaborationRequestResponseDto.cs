using System;

namespace mzansi_builds_api.DTOs.Collaboration
{
    public class CollaborationRequestResponseDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string RequesterUserId { get; set; } = string.Empty;

        // We'll also return the Requester's Name for the UI later
        public string RequesterName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // String version of enum
        public DateTime CreatedAt { get; set; }
    }
}