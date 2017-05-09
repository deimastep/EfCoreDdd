namespace PurchaseApproval.Domain
{
    using System;

    public interface IDecision
    {
        int Number { get; }

        DateTime CreatedAt { get; }

        DateTime ValidTill { get; }

        string Answer { get; }
    }
}