namespace PurchaseApproval.Domain
{
    public class ApprovalData : IApprovalData
    {
        internal ApprovalData(string customerId, string data)
        {
            CustomerId = customerId;
            Data = data;
        }

        // LEAKY: ORM requirement for default ctor
        // ReSharper disable once UnusedMember.Local
        private ApprovalData()
        {
        }

        public string CustomerId { get; protected set; }

        public string Data { get; protected set; }
    }
}