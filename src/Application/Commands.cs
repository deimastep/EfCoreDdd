﻿namespace PurchaseApproval.Application
{
    using System;
    using System.Threading.Tasks;

    public class Commands : ICommands
    {
        private readonly Domain.IApprovalRepository _repo;

        public Commands(Domain.IApprovalRepository repo)
        {
            _repo = repo;
        }

        public async Task Apply(ApplyRequest request)
        {
            var approval = new Domain.PurchaseApproval(Guid.NewGuid(), DateTime.UtcNow);
            _repo.Add(approval);
            await _repo.Save();
        }

        public async Task NewDecision(DecisionAnswer decision)
        {
            var approval = await _repo.GetById(decision.ApprovalId);
            approval.NewDecision(decision.Answer);
            await _repo.Save();
        }
    }
}
