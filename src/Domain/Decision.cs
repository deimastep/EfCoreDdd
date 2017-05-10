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

        // LEAKY: ORM requirement for default ctor
        // ReSharper disable once UnusedMember.Local
        private Decision()
        {
        }

        public int Number { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime ValidTill { get; protected set; }

        public string Answer { get; protected set; }
    }
}
