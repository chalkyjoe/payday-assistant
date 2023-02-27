namespace AutoBanker;

public record StarlingBalance
{
    public StarlingAmount EffectiveBalance { get; set; }
}
public class StarlingAmount
{
    public string currency { get; set; }
    public int minorUnits { get; set; }
    public decimal AmountInPounds => minorUnits / 100m;
}