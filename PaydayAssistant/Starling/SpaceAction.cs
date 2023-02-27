namespace AutoBanker;

public record SpaceAction
{
    public string Name { get; set; }
    public string AccountId { get; set; }
    public string Id {get;set;}
    public decimal Amount {get;set;}
    public string Transtype {get;set;}
    public bool Selected {get;set;}
    public void ToggleSelected(decimal remainingBalance)
    {
        if (!Selected && remainingBalance - Amount <= 0)
            return;
        Selected = !Selected;
    }
}