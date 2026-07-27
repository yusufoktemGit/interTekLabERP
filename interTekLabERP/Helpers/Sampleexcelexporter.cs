using ClosedXML.Excel;
using interTekLabERP.Entities;

namespace interTekLabERP.Helpers;

public static class SampleExcelExporter
{
    public static byte[] Build(IEnumerable<SampleRequest> samples)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Numuneler");

        string[] headers =
           {
            "Takip No", "Teklif No", "Müşteri", "Ürün", "Analiz Bilgisi",
            "Kabul Tarihi", "Durum", "Hizmet Alınan", "Kargo Firması",
            "Satış Fiyatı", "Satış Para Birimi", "Alış Fiyatı", "Alış Para Birimi",
            "Kargo Maliyeti", "Kâr", "Fatura Onaylı", "Çıkış Tarihi",
            "Hedef Süre (İş Günü)", "Hedef Tarih", "Gecikme (Gün)"
        };

        for (int i = 0; i < headers.Length; i++)
            ws.Cell(1, i + 1).Value = headers[i];

        var head = ws.Row(1);
        head.Style.Font.Bold = true;
        head.Style.Fill.BackgroundColor = XLColor.FromHtml("#212529");
        head.Style.Font.FontColor = XLColor.White;

        int r = 2;

        foreach (var s in samples)
        {
            ws.Cell(r, 1).Value = s.TrackingNo;
            ws.Cell(r, 2).Value = s.OfferNo;
            ws.Cell(r, 3).Value = s.CustomerName;
            ws.Cell(r, 4).Value = s.ProductName;
            ws.Cell(r, 5).Value = s.AnalysisInfo;

            ws.Cell(r, 6).Value = s.SampleAcceptDate;
            ws.Cell(r, 6).Style.NumberFormat.Format = "dd.MM.yyyy";

            ws.Cell(r, 7).Value = s.Status?.StatusName ?? "-";
            ws.Cell(r, 8).Value = s.ServicePurchasedFrom ?? "";
            ws.Cell(r, 9).Value = s.CargoCompany ?? "";

            if (s.SalesPrice.HasValue) ws.Cell(r, 10).Value = s.SalesPrice.Value;
            ws.Cell(r, 11).Value = s.SalesCurrency ?? "";

            if (s.PurchasePrice.HasValue) ws.Cell(r, 12).Value = s.PurchasePrice.Value;
            ws.Cell(r, 13).Value = s.PurchaseCurrency ?? "";

            if (s.CargoCost.HasValue) ws.Cell(r, 14).Value = s.CargoCost.Value;
            if (s.Profit.HasValue) ws.Cell(r, 15).Value = s.Profit.Value;

            ws.Cell(r, 16).Value = s.InvoiceApproved == true ? "Evet" : "Hayır";

            if (s.ExitDate.HasValue)
            {
                ws.Cell(r, 17).Value = s.ExitDate.Value;
                ws.Cell(r, 17).Style.NumberFormat.Format = "dd.MM.yyyy";
            }

            // 18: Hedef süre (iş günü)
            if (s.TargetDays.HasValue)
                ws.Cell(r, 18).Value = s.TargetDays.Value;

            // 19: Hedef tarih
            if (s.TargetDate.HasValue)
            {
                ws.Cell(r, 19).Value = s.TargetDate.Value;
                ws.Cell(r, 19).Style.NumberFormat.Format = "dd.MM.yyyy";
            }

            // 20: Gecikme (gün) - iptal edilenler hariç
            if (s.TargetDate.HasValue && s.StatusId != SampleStatusIds.Cancelled)
            {
                if (s.ExitDate.HasValue)
                {
                    // kapanmış iş: çıkış - hedef
                    int diff = (s.ExitDate.Value.Date - s.TargetDate.Value.Date).Days;
                    ws.Cell(r, 20).Value = diff > 0 ? diff : 0;
                }
                else
                {
                    // devam eden iş: bugün - hedef (sadece geçmişse)
                    int diff = (DateTime.Now.Date - s.TargetDate.Value.Date).Days;
                    ws.Cell(r, 20).Value = diff > 0 ? diff : 0;
                }
            }
            r++;
        }

        ws.Columns().AdjustToContents();
        ws.SheetView.FreezeRows(1);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}