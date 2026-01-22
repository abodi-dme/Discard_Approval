using System.Collections.Generic;

namespace WebApp_Anti.Models
{
    public class DiscardApprovalTableViewModel
    {
        public string Stage { get; set; } = "manager";
        public IEnumerable<DiscardApprovalInput> Items { get; set; } = new List<DiscardApprovalInput>();
        public PagerViewModel Pager { get; set; } = new PagerViewModel();

        public List<string> ProductOptions { get; set; } = new();
        public List<string> AssetTagOptions { get; set; } = new();
        public List<string> StatusOptions { get; set; } = new();
        public List<string> ManufacturerOptions { get; set; } = new();
        public List<string> ModelOptions { get; set; } = new();
        public List<string> SerialNumberOptions { get; set; } = new();
        public List<string> WarehouseOptions { get; set; } = new();
        public List<string> DiscardReasonOptions { get; set; } = new();

        public string? FilterProduct { get; set; }
        public string? FilterAssetTag { get; set; }
        public string? FilterStatus { get; set; }
        public string? FilterManufacturer { get; set; }
        public string? FilterModel { get; set; }
        public string? FilterSerialNumber { get; set; }
        public string? FilterWarehouse { get; set; }
        public string? FilterDiscardReason { get; set; }
    }
}
