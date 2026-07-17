using interTekLabERP.Data;
using interTekLabERP.Entities;
using InterTekLabERP.Entities.Domain;
using Microsoft.EntityFrameworkCore;

namespace interTekLabERP.Business.Services;

public class SampleHistoryService : ISampleHistoryService
{
    private readonly ApplicationDbContext _context;

    public SampleHistoryService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(int sampleId, string actionType, string description, int userId)
    {
        var history = new SampleHistory
        {
            SampleId = sampleId,
            ActionType = actionType,
            Description = description,
            UserId = userId,
            CreatedDate = DateTime.Now
        };

        _context.SampleHistories.Add(history);

        _context.SaveChanges();
    }

    public List<SampleHistory> GetBySampleId(int sampleId)
    {
        return _context.SampleHistories
            .Include(x => x.User)
            .Where(x => x.SampleId == sampleId)
            .OrderByDescending(x => x.CreatedDate)
            .Take(50)
            .ToList();
    }
}