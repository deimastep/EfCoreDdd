namespace PurchaseApproval.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PurchaseApproval2 : IAggregateRoot<Guid>
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public string Status { get; protected set; }

        public IReadOnlyCollection<IDecision> Decisions => DecisionsInternal;

        protected List<Decision> DecisionsInternal { get; }

        private PurchaseApproval2()
        {
            DecisionsInternal = new List<Decision>();
        }

        public PurchaseApproval2(Guid id, DateTime createdAt) : this()
        {
            Id = id;
            CreatedAt = createdAt;
            Status = "InProgress";
        }

        public void Close()
        {
            Status = "Closed";
        }

        public void NewDecision(string answer)
        {
            var number = DecisionsInternal.Count != 0 ? DecisionsInternal.Max(a => a.Number) + 1 : 1;
            DecisionsInternal.Add(new Decision(number, DateTime.Now, DateTime.Now.AddDays(30), answer));
        }

        public bool CancelDecision(int number)
        {
            var item = DecisionsInternal.FirstOrDefault(d => d.Number == number);
            if (item == null)
            {
                return false;
            }
            DecisionsInternal.Remove(item);
            return true;
        }
    }
}
