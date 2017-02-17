namespace PurchaseApproval.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;

    public class Request : IAggregateRoot
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        [MaxLength(20)]
        public string Status { get; protected set; }

        public IEnumerable<IApproval> Approvals => _approvals;

        protected ICollection<Approval> _approvals { get; }

        private Request()
        {
            _approvals = new Collection<Approval>();
        }

        public Request(Guid id, DateTime createdAt) : this()
        {
            Id = id;
            CreatedAt = createdAt;
            Status = "Created";
        }

        public void InProgress()
        {
            Status = "InProgress";
        }

        public void NewApproval(string decision)
        {
            var number = _approvals.Count != 0 ? _approvals.Max(a => a.Number) + 1 : 1;
            _approvals.Add(new Approval
            {
                Number = number,
                CreatedAt = DateTime.Now,
                Decision = decision,
                ValidTill = DateTime.Now.AddDays(30)
            });
        }
    }
}
