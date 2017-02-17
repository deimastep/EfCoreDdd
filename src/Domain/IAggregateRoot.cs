namespace PurchaseApproval.Domain
{
    using System;

    public interface IAggregateRoot : IEntity
    {
        Guid Id { get; }
    }
}
