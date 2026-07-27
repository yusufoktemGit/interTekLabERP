using interTekLabERP.Entities;

namespace interTekLabERP.ViewModels;

public class DashboardViewModel
{
    public int TotalSamples { get; set; }

    public int PendingSamples { get; set; }

    public int InAnalysisSamples { get; set; }

    public int ShippedSamples { get; set; }

    public int CompletedSamples { get; set; }
    public int CancelledSamples { get; set; }

    public int InvoicedSamples { get; set; }

    public int TotalUsers { get; set; }

    public int ActiveUsers { get; set; }

    public List<SampleRequest> RecentSamples { get; set; }
        = new();
}