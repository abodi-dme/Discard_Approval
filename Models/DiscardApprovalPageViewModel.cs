namespace WebApp_Anti.Models
{
    public class DiscardApprovalPageViewModel
    {
        public DiscardApprovalTableViewModel ManagerTable { get; set; } = new DiscardApprovalTableViewModel { Stage = "manager" };
        public DiscardApprovalTableViewModel FinalTable { get; set; } = new DiscardApprovalTableViewModel { Stage = "final" };
    }
}
