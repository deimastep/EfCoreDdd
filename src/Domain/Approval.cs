namespace PurchaseApproval.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Approval : IApproval
    {
        public int Number { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ValidTill { get; set; }

        [MaxLength(20)]
        public string Decision { get; set; }

        //public RequestData RequestData { get; set; }

    }

    public class RequestData
    {
        public string Data { get; protected set; }
    }

    public interface IApproval : IEntity
    {
        int Number { get; }

        DateTime CreatedAt { get; }

        DateTime ValidTill { get; }

        string Decision { get; }
    }
}
