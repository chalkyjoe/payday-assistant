namespace AutoBanker;

public record SavingsGoalsResponse
{
    public IEnumerable<SavingsGoal> SavingsGoalList { get; set; }
}
public record SavingsGoal
{
    public string SavingsGoalUid { get; set; }
    public string Name { get; set; }
    public StarlingAmount? Target { get; set; }
    public StarlingAmount? TotalSaved { get; set; }
    public decimal SavedPercentage { get; set; }

    public decimal RemainingTarget =>
        Target?.minorUnits - TotalSaved?.minorUnits ?? 0m;
}