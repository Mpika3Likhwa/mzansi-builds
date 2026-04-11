using System.ComponentModel.DataAnnotations;

public class CollaborationRequest
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string RequesterUserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}