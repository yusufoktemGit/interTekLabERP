namespace interTekLabERP.Entities;

public class BulkSampleRow
{
    public string ProductName { get; set; } = string.Empty;

    public string AnalysisInfo { get; set; } = string.Empty;

    public List<string> AnalysisTests { get; set; } = new();   

    public string? ServicePurchasedFrom { get; set; }

    public int? TargetDays { get; set; }
}