namespace PurchaseApproval.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Approval : IAggregateRoot<Guid>
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        [MaxLength(20)]
        public string Status { get; protected set; }

        public IEnumerable<Decision> Decisions => _decisions;

        private readonly List<Decision> _decisions;

        private Approval()
        {
            _decisions = new List<Decision>();
        }

        public Approval(Guid id, DateTime createdAt) : this()
        {
            Id = id;
            CreatedAt = createdAt;
            Status = "Created";
        }

        public void InProgress()
        {
            Status = "InProgress";
        }

        public void NewDecision(string answer)
        {
            var number = _decisions.Count != 0 ? _decisions.Max(a => a.Number) + 1 : 1;
            _decisions.Add(new Decision(number, DateTime.Now, DateTime.Now.AddDays(30), answer));
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
    }
}
