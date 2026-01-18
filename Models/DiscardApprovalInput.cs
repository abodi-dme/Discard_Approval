using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApp_Anti.Models
{
    [Keyless]
    [Table("Discard Approval Input", Schema = "dbo")]
    public class DiscardApprovalInput
    {
        public string? ID { get; set; }
        
        public string? Product { get; set; }

        [Column("Master ID")]
        public string? MasterId { get; set; }

        [Column("Asset Tag")]
        public string? AssetTag { get; set; }

        public string? Lost { get; set; }

        public string? Status { get; set; }

        [Column("Warehouse Name")]
        public string? WarehouseName { get; set; }

        public string? Manufacturer { get; set; }

        public string? ManufacturerSerialNumber { get; set; }

        public string? Model { get; set; }

        public string? ModelNumber { get; set; }

        public string? UnitCost { get; set; }

        [Column("Depreciated Value")]
        public string? DepreciatedValue { get; set; }

        [Column("Is Under Warranty")]
        public string? IsUnderWarranty { get; set; }

        [Column("Discard Notes")]
        public string? DiscardNotes { get; set; }

        [Column("Last Updated")]
        public DateTime? LastUpdated { get; set; }

        [Column("Updated By")]
        public string? UpdatedBy { get; set; }

        [Column("Purchase Date")]
        public string? PurchaseDate { get; set; }

        [Column("PO #")]
        public string? PONumber { get; set; }

        [Column("Last Cleaned")]
        public string? LastCleaned { get; set; }

        [Column("Cleaned By")]
        public string? CleanedBy { get; set; }

        [Column("Last Tested")]
        public string? LastTested { get; set; }

        [Column("Tested By")]
        public string? TestedBy { get; set; }

        [Column("Last Repaired")]
        public string? LastRepaired { get; set; }

        [Column("Repaired By")]
        public string? RepairedBy { get; set; }

        public string? SystemUpdatedDate { get; set; }

        public string? Approved { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
