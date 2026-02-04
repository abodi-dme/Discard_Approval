namespace WebApp_Anti.Models
{
    public class DiscardApprovalPageViewModel
    {
        public DiscardApprovalTableViewModel ManagerTable { get; set; } = new DiscardApprovalTableViewModel { Stage = "manager" };
        public DiscardApprovalTableViewModel FinalTable { get; set; } = new DiscardApprovalTableViewModel { Stage = "final" };
        public DiscardApprovalTableViewModel ApprovedTable { get; set; } = new DiscardApprovalTableViewModel { Stage = "approved" };

        public decimal TotalDiscardCost { get; set; }
        public decimal ApprovedDiscardCost { get; set; }
    }
}
