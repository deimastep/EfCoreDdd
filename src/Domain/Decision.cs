namespace PurchaseApproval.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Decision
    {
        public Decision(int number, DateTime createdAt, DateTime validTill, string answer)
        {
            Number = number;
            CreatedAt = createdAt;
            ValidTill = validTill;
            Answer = answer;
        }

        // LEAKY: ORM requirement
        private Decision()
        {
        }

        public int Number { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime ValidTill { get; protected set; }

        [MaxLength(20)]
        public string Answer { get; protected set; }
    }
}
