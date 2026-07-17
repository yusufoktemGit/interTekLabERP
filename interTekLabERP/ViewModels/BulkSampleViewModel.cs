using interTekLabERP.Entities;

namespace interTekLabERP.ViewModels;

public class BulkSampleViewModel
{
    public string OfferNo { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public List<BulkSampleRow> Rows { get; set; } = new();
}