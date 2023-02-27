using System.Net.Http.Headers;

namespace AutoBanker
{
    public sealed class StarlingService
    {
        private readonly StarlingApi _starlingApi;

        public StarlingService(StarlingApi starlingApi)
        {
            _starlingApi = starlingApi;
        }

        public async Task<decimal> GetBalance(string accountId)
        {
            var balance = await _starlingApi.GetBalance(accountId);
            return balance.EffectiveBalance.AmountInPounds;
        }

        public async Task<IEnumerable<SpaceAction>> GetSavingsGoalsActions(string accountId)
        {
            var getBalance = await _starlingApi.GetBalance(accountId);
            var balance = getBalance.EffectiveBalance.minorUnits;
            var savingsGoals = await _starlingApi.GetSavingsGoals(accountId);
            return GetSavingsGoalActions(accountId, savingsGoals.SavingsGoalList, balance)
                .OrderByDescending(m => m.Selected)
                .ThenBy(m => m.Amount);
        }

        public async Task<bool> TransferToSpace(SpaceAction spaceAction)
        {
            var response = await _starlingApi.TransferToSpace(spaceAction.AccountId, spaceAction.Id, spaceAction.Amount);
            return response.Success;
        }

        private static IEnumerable<SpaceAction> GetSavingsGoalActions(string accountId, IEnumerable<SavingsGoal>? savingsGoals, decimal balance)
        {
            foreach (var savingsGoal in savingsGoals.OrderBy(m => m.RemainingTarget))
            {
                if (savingsGoal.Target is null)
                    continue;
                var remaining = savingsGoal.RemainingTarget > 0 ? savingsGoal.RemainingTarget : savingsGoal.Target.minorUnits;
                var isSelected = balance - remaining > 0 && savingsGoal.RemainingTarget != 0;
                if (isSelected)
                    balance -= remaining;
                
                yield return new SpaceAction()
                {
                    Name = savingsGoal.Name,
                    AccountId = accountId,
                    Amount = remaining / 100,
                    Id = savingsGoal.SavingsGoalUid,
                    Selected = isSelected,
                    Transtype = "Deposit",
                };
            }
        }

        public async Task<string> GetFirstAccount()
        {
            var accounts = await _starlingApi.GetAccounts();
            return accounts.Accounts.FirstOrDefault()?.AccountUid ?? throw new InvalidOperationException();
        }
    }
}
