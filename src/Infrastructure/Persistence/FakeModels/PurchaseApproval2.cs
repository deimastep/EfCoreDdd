namespace PurchaseApproval.Infrastructure.Persistence.FakeModels
{
    using System;
    using System.Collections.Generic;

    public class PurchaseApproval2
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Decision2> Decisions { get; set; }

        public ApprovalData2 Data { get; set; }
    }

    public class Decision2
    {
        public int Id { get; set; }

        public Guid ApprovalId { get; set; }

        public int Number { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ValidTill { get; set; }

        public string Answer { get; set; }
    }

    public class ApprovalData2
    {
        public int Id { get; set; }

        public Guid ApprovalId { get; set; }

        public string CustomerId { get; set; }

        public string Data { get; set; }
    }
}
