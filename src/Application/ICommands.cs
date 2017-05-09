namespace PurchaseApproval.Application
{
    using System.Threading.Tasks;

    public interface ICommands
    {
        Task Apply(ApplyRequest request);

        Task NewDecision(DecisionAnswer decision);
    }
}