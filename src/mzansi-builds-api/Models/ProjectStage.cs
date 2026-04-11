using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mzansi_builds_api.Models
{
    public class ProjectStage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Added to fulfill the requirement: "including stage and support required"
        [Required]
        public string SupportRequired { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        // Foreign Key
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        [JsonIgnore]
        public Project? Project { get; set; }
    }
}