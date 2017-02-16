namespace PurchaseApproval.ServiceHost
{
    using System;

    public interface IPurchaseApprovalWindowsService
    {
        void Start();

        void Stop();

        Uri ServiceUri { get; }
    }
}
