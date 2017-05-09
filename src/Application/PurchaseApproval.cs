namespace PurchaseApproval.Application
{
    using System;

    public class PurchaseApproval
    {
        public string Status { get; set; }

        public Decision Decision { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
