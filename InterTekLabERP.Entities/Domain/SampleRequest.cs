namespace interTekLabERP.Entities;

public class SampleRequest
{
    public int Id { get; set; }

    public string TrackingNo { get; set; } = string.Empty;

    public string OfferNo { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string AnalysisInfo { get; set; } = string.Empty;

    public DateTime SampleAcceptDate { get; set; }

    public int StatusId { get; set; }

    public Status? Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    public string? SampleCode { get; set; }

    // Finans
    public decimal? SalesPrice { get; set; }

    public string? SalesCurrency { get; set; }

    public decimal? PurchasePrice { get; set; }

    public string? PurchaseCurrency { get; set; }

    public decimal? CargoCost { get; set; }

    public bool? InvoiceApproved { get; set; }

    public decimal? Profit { get; set; }
    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? ExitDate { get; set; }
    public string? CancelReason { get; set; }

    public DateTime? TargetDate { get; set; }

    public int? TargetDays { get; set; }

    public User? UpdatedByUser { get; set; }
    public string? ServicePurchasedFrom { get; set; }
    public string? CargoCompany { get; set; }
}