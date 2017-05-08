namespace PurchaseApproval.Domain
{
    public interface IAggregateRoot<out TId> : IEntity
    {
        TId Id { get; }
    }
}
