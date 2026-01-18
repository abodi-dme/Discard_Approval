using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApp_Anti.Models
{
    [Keyless]
    [Table("SERP_PRODUCT_INSTANCES", Schema = "dbo")]
    public class SerpProductInstance
    {
        public string? Product { get; set; }

        [Column("Asset Tag")]
        public string? AssetTag { get; set; }

        public string? Status { get; set; }

        public string? Manufacturer { get; set; }

        public string? Model { get; set; }

        [Column("Warehouse Name")]
        public string? WarehouseName { get; set; }

        // Placeholders for columns with unknown DB names
        [NotMapped]
        public string? Type { get; set; }

        [NotMapped]
        // Display name can be handled in View or with [Display]
        public string? MfrSerial { get; set; } 

        [NotMapped]
        public string? DiscardReason { get; set; }

        [NotMapped]
        public string? MgrWarehouse { get; set; }

        [NotMapped]
        public string? MgrDfo { get; set; }

        [NotMapped]
        public string? MgrVp { get; set; }
    }
}
