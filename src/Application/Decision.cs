namespace PurchaseApproval.Application
{
    using System;

    public class Decision
    {
        public string Answer { get; set; }

        public DateTime ValidTill { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}