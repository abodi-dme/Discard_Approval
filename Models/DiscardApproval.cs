using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp_Anti.Models
{
    [Table("DiscardApprovals", Schema = "dbo")]
    public class DiscardApproval
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string AssetTag { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Product { get; set; }

        [MaxLength(10)]
        public string? Approved { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? Notes { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
