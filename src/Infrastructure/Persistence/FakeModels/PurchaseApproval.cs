namespace PurchaseApproval.Infrastructure.Persistence.FakeModels
{
    using System;
    using System.Collections.Generic;

    public class PurchaseApproval
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Decision> Decisions { get; set; }

        public ApprovalData Data { get; set; }
    }

    public class Decision
    {
        public Guid ApprovalId { get; set; }

        public int Number { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ValidTill { get; set; }

        public string Answer { get; set; }
    }

    public class ApprovalData
    {
        public Guid Id { get; set; }

        public string CustomerId { get; set; }

        public string Data { get; set; }
    }
}
