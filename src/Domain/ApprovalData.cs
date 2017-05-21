namespace PurchaseApproval.Domain
{
    public class ApprovalData : IApprovalData
    {
        internal ApprovalData(string customerId, string data)
        {
            CustomerId = customerId;
            Data = data;
        }

        // LEAKY: ORM requirement for default constructor
        // ReSharper disable once UnusedMember.Local
        private ApprovalData()
        {
        }

        public string CustomerId { get; protected internal set; }

        public string Data { get; protected internal set; }
    }
}