namespace PurchaseApproval.Domain
{
    public interface IApprovalData
    {
        string CustomerId { get; }

        string Data { get; }
    }
}