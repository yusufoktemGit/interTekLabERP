using interTekLabERP.Business.Services;
using interTekLabERP.Entities;
using interTekLabERP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using interTekLabERP.Helpers;

namespace interTekLabERP.Controllers;

[Authorize]
public class SampleController : Controller
{
    private readonly ISampleService _sampleService;
    private readonly IStatusService _statusService;
    private readonly IQrCodeService _qrCodeService;
    private readonly IBarcodeService _barcodeService;
    private readonly ISampleHistoryService _historyService;
    private readonly ITestCardService _testCardService;

    public SampleController(
        ISampleService sampleService,
        IStatusService statusService,
        IQrCodeService qrCodeService,
        IBarcodeService barcodeService,
        ISampleHistoryService historyService,
        ITestCardService testCardService)
    {
        _sampleService = sampleService;
        _statusService = statusService;
        _qrCodeService = qrCodeService;
        _barcodeService = barcodeService;
        _historyService = historyService;
        _testCardService = testCardService;
    }

    public IActionResult Index(int? statusId, DateTime? startDate, DateTime? endDate, string? search)
    {
        var model = _sampleService.GetForExport(statusId, startDate, endDate, search);

        // seçili filtreleri forma geri yansıt
        ViewBag.SelectedStatus = statusId;
        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
        ViewBag.Search = search;

        return View(model);
    }
    public IActionResult ExportExcel(int? statusId, DateTime? startDate, DateTime? endDate, string? search)
    {
        var samples = _sampleService.GetForExport(statusId, startDate, endDate, search);

        var bytes = SampleExcelExporter.Build(samples);

        var fileName = $"Numuneler_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    public IActionResult BulkCreate()
    {
        ViewBag.TestCards = _testCardService.GetActive();

        var model = new BulkSampleViewModel
        {
            Rows = new List<BulkSampleRow> { new BulkSampleRow() }  // 1 boş satırla başla
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult BulkCreate(BulkSampleViewModel model)
    {
        bool hasAnyRow = model.Rows != null &&
                         model.Rows.Any(r => !string.IsNullOrWhiteSpace(r.ProductName));

        if (string.IsNullOrWhiteSpace(model.OfferNo) ||
            string.IsNullOrWhiteSpace(model.CustomerName) ||
            !hasAnyRow)
        {
            TempData["Error"] = "Teklif No, Müşteri Adı ve en az bir ürün satırı zorunludur.";
            ViewBag.TestCards = _testCardService.GetActive();

            if (model.Rows == null || model.Rows.Count == 0)
            {
                model.Rows = new List<BulkSampleRow> { new BulkSampleRow() };
            }

            return View(model);
        }

        var filled = model.Rows.Where(r => !string.IsNullOrWhiteSpace(r.ProductName));

        if (filled.Any(r => r.AnalysisTests == null || r.AnalysisTests.Count == 0))
        {
            TempData["Error"] = "Her numune satırında en az bir analiz/test seçilmelidir.";
            ViewBag.TestCards = _testCardService.GetActive();
            return View(model);
        }

        if (filled.Any(r => r.TargetDays == null || r.TargetDays < 1 || r.TargetDays > 99))
        {
            TempData["Error"] = "Her numune satırında hedef süre 1-99 iş günü arasında olmalıdır.";
            ViewBag.TestCards = _testCardService.GetActive();
            return View(model);
        }

        var createdBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _sampleService.AddBulk(model.OfferNo, model.CustomerName, model.Rows, createdBy);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Create()
    {
        ViewBag.NextTrackingNo = _sampleService.GenerateTrackingNo();
        ViewBag.TestCards = _testCardService.GetActive();

        return View();
    }
    [HttpPost]
    public IActionResult Create(SampleRequest model, string[] analysisTests)
    {
        model.AnalysisInfo = analysisTests != null && analysisTests.Length > 0
            ? string.Join(" | ", analysisTests)
            : string.Empty;

        if (string.IsNullOrWhiteSpace(model.AnalysisInfo))
            ModelState.AddModelError("AnalysisInfo", "En az bir analiz/test seçiniz.");

        if (model.TargetDays == null || model.TargetDays < 1 || model.TargetDays > 99)
            ModelState.AddModelError("TargetDays", "Hedef süre 1-99 iş günü arasında olmalıdır.");

        if (!ModelState.IsValid)
        {
            ViewBag.NextTrackingNo = _sampleService.GenerateTrackingNo();
            ViewBag.TestCards = _testCardService.GetActive();
            return View(model);
        }

        model.CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _sampleService.Add(model);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Detail(int id)
    {
        var model = _sampleService.GetById(id);

        if (model == null)
        {
            return NotFound();
        }

        ViewBag.Statuses = _statusService.GetAll()
          .Where(x => x.Id != SampleStatusIds.Cancelled)
          .ToList();

        ViewBag.History = _historyService.GetBySampleId(id);

        ViewBag.BarcodeImage = _barcodeService.Generate(model.SampleCode ?? model.TrackingNo);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(int sampleId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            TempData["Error"] = "İptal nedeni girilmelidir.";
            return RedirectToAction(nameof(Detail), new { id = sampleId });
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _sampleService.Cancel(sampleId, userId, reason);

        return RedirectToAction(nameof(Detail), new { id = sampleId });
    }

    [HttpPost]
    //[Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _sampleService.Delete(id);
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Edit(int id)
    {
        var model = _sampleService.GetById(id);

        if (model == null)
        {
            return NotFound();
        }
        ViewBag.TestCards = _testCardService.GetActive();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(SampleRequest model, string[] analysisTests)
    {

        if (model.TargetDays == null || model.TargetDays < 1 || model.TargetDays > 99)
        {
            TempData["Error"] = "Hedef süre 1-99 iş günü arasında olmalıdır.";
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        var entity = _sampleService.GetById(model.Id);

        if (entity == null)
        {
            return NotFound();
        }

        entity.OfferNo = model.OfferNo;
        entity.CustomerName = model.CustomerName;
        entity.ProductName = model.ProductName;
        entity.ServicePurchasedFrom = model.ServicePurchasedFrom;

        entity.AnalysisInfo = analysisTests != null && analysisTests.Length > 0
            ? string.Join(" | ", analysisTests)
            : string.Empty;

        // hedef süre değiştiyse hedef tarihi kabul tarihine göre yeniden hesapla
        entity.TargetDays = model.TargetDays;

        if (model.TargetDays.HasValue && model.TargetDays.Value > 0)
        {
            entity.TargetDate = _sampleService.CalculateTargetDate(
                entity.SampleAcceptDate,
                model.TargetDays.Value);
        }

        entity.UpdatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        entity.UpdatedDate = DateTime.Now;

        _sampleService.Update(entity);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult UpdateStatus(
    int sampleId,
    int statusId,
    string? cargoCompany)
    {
        try
        {
            var userId = int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            _sampleService.UpdateStatus(
                sampleId,
                statusId,
                userId,
                cargoCompany);

            return RedirectToAction(
                nameof(Detail),
                new { id = sampleId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                nameof(Detail),
                new { id = sampleId });
        }
    }

    public IActionResult PrintLabel(int id)
    {
        var sample = _sampleService.GetById(id);

        if (sample == null)
            return NotFound();

        ViewBag.QrImage =
            _qrCodeService.Generate(
                sample.SampleCode ?? sample.TrackingNo);

        return View(sample);
    }

    public IActionResult PrintBarcode(int id)
    {
        var sample = _sampleService.GetById(id);

        if (sample == null)
        {
            return NotFound();
        }

        ViewBag.BarcodeImage =
            _barcodeService.Generate(
                sample.SampleCode ?? sample.TrackingNo);

        return View(sample);
    }

}