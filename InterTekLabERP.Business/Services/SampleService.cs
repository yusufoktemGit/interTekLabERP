using interTekLabERP.Data;
using interTekLabERP.Entities;
using Microsoft.EntityFrameworkCore;


namespace interTekLabERP.Business.Services;

public class SampleService : ISampleService
{
    private readonly ApplicationDbContext _context;
    private readonly ISampleHistoryService _historyService;

    public SampleService(ApplicationDbContext context, ISampleHistoryService historyService)
    {
        _context = context;
        _historyService = historyService;
    }

    public List<SampleRequest> GetAll()
    {
        return _context.SampleRequests
            .Include(x => x.Status)
            .OrderByDescending(x => x.Id)
            .ToList();
    }

    public SampleRequest? GetById(int id)
    {
        return _context.SampleRequests
            .Include(x => x.Status)
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser)
            .FirstOrDefault(x => x.Id == id);
    }
    public List<SampleRequest> GetForExport(int? statusId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.SampleRequests
            .Include(x => x.Status)
            .AsQueryable();

        if (statusId.HasValue)
            query = query.Where(x => x.StatusId == statusId.Value);

        if (startDate.HasValue)
            query = query.Where(x => x.SampleAcceptDate >= startDate.Value);

        if (endDate.HasValue)
        {
            var end = endDate.Value.Date.AddDays(1); // bitiş gününü tam kapsa
            query = query.Where(x => x.SampleAcceptDate < end);
        }

        return query
            .OrderByDescending(x => x.SampleAcceptDate)
            .ToList();
    }

    public void AddBulk(string offerNo, string customerName, List<BulkSampleRow> rows, int createdBy)
    {
        foreach (var row in rows)
        {
            // boş satırları atla (ürün adı yoksa kaydetme)
            if (string.IsNullOrWhiteSpace(row.ProductName))
            {
                continue;
            }

            var sample = new SampleRequest
            {
                OfferNo = offerNo,
                CustomerName = customerName,
                ProductName = row.ProductName,
                AnalysisInfo = row.AnalysisTests != null && row.AnalysisTests.Count > 0
                      ? string.Join(" | ", row.AnalysisTests)
                      : row.AnalysisInfo,
                ServicePurchasedFrom = row.ServicePurchasedFrom,
                TargetDays = row.TargetDays,
                CreatedBy = createdBy
            };

            // mevcut tekli Add mantığını kullan:
            // takip no üretir, tarihleri ve durumu ayarlar, geçmişe kaydeder
            Add(sample);
        }
    }

    public void Add(SampleRequest sampleRequest)
    {
        var now = DateTime.Now;

        string trackingNo = GenerateTrackingNo();

        sampleRequest.SampleAcceptDate = now;
        sampleRequest.CreatedDate = now;

        sampleRequest.TrackingNo = trackingNo;

        sampleRequest.SampleCode = trackingNo;

        sampleRequest.StatusId = SampleStatusIds.Registered;

        // hedef tarihi hesapla
        if (sampleRequest.TargetDays.HasValue && sampleRequest.TargetDays.Value > 0)
        {
            sampleRequest.TargetDate = CalculateTargetDate(now, sampleRequest.TargetDays.Value);
        }

        _context.SampleRequests.Add(sampleRequest);

        _context.SaveChanges();

        _historyService.Add(
            sampleRequest.Id,
            "CREATE",
            $"Numune oluşturuldu. Takip No: {trackingNo}",
            sampleRequest.CreatedBy ?? 0);
    }

    public string GenerateTrackingNo()
    {
        int year = DateTime.Now.Year;

        int lastId = _context.SampleRequests
            .OrderByDescending(x => x.Id)
            .Select(x => x.Id)
            .FirstOrDefault();

        return $"ITL-{year}-{(lastId + 1):00000}";
    }

    public DateTime CalculateTargetDate(DateTime acceptDate, int workingDays)
    {
        var current = acceptDate.Date;
        int counted = 0;

        // 12:00'dan önce girildiyse ve o gün iş günüyse, kabul günü 1. gün sayılır
        if (acceptDate.Hour < 12 && IsWorkingDay(current))
        {
            counted = 1;
        }

        while (counted < workingDays)
        {
            current = current.AddDays(1);

            if (IsWorkingDay(current))
            {
                counted++;
            }
        }

        return current;
    }

    private static bool IsWorkingDay(DateTime date)
    {
        // Resmi tatiller eklenecekse SADECE burası genişletilir
        return date.DayOfWeek != DayOfWeek.Saturday
            && date.DayOfWeek != DayOfWeek.Sunday;
    }

    public void Update(SampleRequest sampleRequest)
    {
        _context.SampleRequests.Update(sampleRequest);

        _context.SaveChanges();

        _historyService.Add(
             sampleRequest.Id,
             "UPDATE",
             "Numune bilgileri güncellendi.",
             sampleRequest.UpdatedBy ?? 0);
    }

    public void Cancel(int sampleId, int userId, string reason)
    {
        var sample = _context.SampleRequests
            .Include(x => x.Status)
            .FirstOrDefault(x => x.Id == sampleId);

        if (sample == null)
        {
            return;
        }

        string oldStatus = sample.Status?.StatusName ?? "";

        sample.StatusId = SampleStatusIds.Cancelled;
        sample.CancelReason = reason;

        _context.SaveChanges();

        _historyService.Add(
            sample.Id,
            "CANCEL",
            $"İş iptal edildi. Önceki durum: {oldStatus}. Neden: {reason}",
            userId);
    }

    public void Delete(int id)
    {
        var sample = _context.SampleRequests
            .FirstOrDefault(x => x.Id == id);

        if (sample == null)
        {
            return;
        }

        var histories = _context.SampleHistories
            .Where(x => x.SampleId == id)
            .ToList();

        _context.SampleHistories.RemoveRange(histories);
        _context.SampleRequests.Remove(sample);
        _context.SaveChanges();
    }


    public void UpdateStatus(
    int sampleId,
    int statusId,
    int userId,
    string? cargoCompany)
    {
        var sample = _context.SampleRequests
            .Include(x => x.Status)
            .FirstOrDefault(x => x.Id == sampleId);

        if (sample == null)
        {
            return;
        }

        string oldStatus =
            sample.Status?.StatusName ?? "";

        var newStatus = _context.Statuses
            .FirstOrDefault(x => x.Id == statusId);

        // Kargoya Verildi kontrolü
        if (newStatus?.StatusName == "Kargoya Verildi"
            && string.IsNullOrWhiteSpace(cargoCompany))
        {
            throw new Exception(
                "Kargoya verildi durumunda kargo firması girilmelidir.");
        }

        if (!string.IsNullOrWhiteSpace(cargoCompany))
        {
            sample.CargoCompany = cargoCompany;
        }

        sample.StatusId = statusId;

        if (statusId == SampleStatusIds.ReportCompleted)
        {
            sample.ExitDate = DateTime.Now;
        }
        _context.SaveChanges();

        var description =
            $"Durum değiştirildi: {oldStatus} → {newStatus?.StatusName}";

        if (!string.IsNullOrWhiteSpace(cargoCompany))
        {
            description +=
                $" | Kargo Firması: {cargoCompany}";
        }

        _historyService.Add(
            sample.Id,
            "STATUS_CHANGE",
            description,
            userId);
    }

}