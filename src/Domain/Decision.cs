namespace PurchaseApproval.Domain
{
    using System;

    public class Decision : IDecision
    {
        internal Decision(int number, DateTime createdAt, DateTime validTill, string answer)
        {
            Number = number;
            CreatedAt = createdAt;
            ValidTill = validTill;
            Answer = answer;
        }

        // LEAKY: ORM requirement for default constructor
        // ReSharper disable once UnusedMember.Local
        private Decision()
        {
        }

        public int Number { get; protected internal set; }

        public DateTime CreatedAt { get; protected internal set; }

        public DateTime ValidTill { get; protected internal set; }

        public string Answer { get; protected internal set; }
    }
}