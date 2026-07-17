using interTekLabERP.Data;
using interTekLabERP.Entities;
using interTekLabERP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace interTekLabERP.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var TotalSamples =  _context.SampleRequests.Count();
        var PendingSamples = _context.SampleRequests.Count(x => x.StatusId == SampleStatusIds.Registered);
        var InAnalysisSamples = _context.SampleRequests.Count(x => x.StatusId == SampleStatusIds.AnalysisInProgress);
        var ShippedSamples = _context.SampleRequests.Count(x => x.StatusId == SampleStatusIds.Shipped);
        var CompletedSamples =  _context.SampleRequests.Count(x => x.StatusId == SampleStatusIds.ReportCompleted);
        var CancelledSamples = _context.SampleRequests.Count(x => x.StatusId == SampleStatusIds.Cancelled);
        var RecentSamples = _context.SampleRequests.Include(x => x.Status).OrderByDescending(x => x.CreatedDate).Take(10).ToList();
        var TotalUsers = _context.Users.Count();
        var ActiveUsers = _context.Users.Count(x => x.IsActive);


        var model = new DashboardViewModel
        {
            TotalSamples = TotalSamples,

            PendingSamples = PendingSamples,

            InAnalysisSamples = InAnalysisSamples,

            ShippedSamples = ShippedSamples,

            CompletedSamples = CompletedSamples,

            CancelledSamples = CancelledSamples,

            TotalUsers = TotalUsers,

            ActiveUsers = ActiveUsers,

            RecentSamples = RecentSamples
        };

        return View(model);
    }
}