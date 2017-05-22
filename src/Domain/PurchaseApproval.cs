namespace PurchaseApproval.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PurchaseApproval : IAggregateRoot<Guid>
    {
        private readonly List<Decision> _decisions = new List<Decision>();

        public PurchaseApproval(Guid id, string customerId, DateTime createdAt) : this()
        {
            Id = id;
            CreatedAt = createdAt;
            Status = "InProgress";
            Data = new ApprovalData(customerId, null);
        }

        // LEAKY: ORM requirement for default constructor
        private PurchaseApproval()
        {
        }

        public Guid Id { get; protected internal set; }

        public string Status { get; protected internal set; }

        public DateTime CreatedAt { get; protected internal set; }

        // Despite that List<> already implement IReadOnlyCollection<>,
        // we use AsReadOnly() here to demonstrate the concept of how EF Core
        // handle fully encapsulated collection by using backing private readonly field
        public IReadOnlyCollection<Decision> Decisions => _decisions.AsReadOnly();

        public ApprovalData Data { get; protected internal set; }

        public void Close()
        {
            Status = "Closed";
        }

        public void NewDecision(string answer)
        {
            var number = _decisions.Count != 0 ? _decisions.Max(a => a.Number) + 1 : 1;
            _decisions.Add(new Decision(number, DateTime.Now, DateTime.Now.AddDays(30), answer));
            Status = "DecisionMade";
        }

        public bool CancelDecision(int number)
        {
            var item = _decisions.FirstOrDefault(d => d.Number == number);
            if (item == null)
            {
                return false;
            }
            _decisions.Remove(item);
            return true;
        }

        public void StupidMethodToRemoveMandatoryData()
        {
            Data = null;
        }
    }
}