using interTekLabERP.Data;
using interTekLabERP.Entities;

namespace interTekLabERP.Business.Services;

public class StatusService : IStatusService
{
    private readonly ApplicationDbContext _context;

    public StatusService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Status> GetAll()
    {
        return _context.Statuses.OrderBy(x => x.Id).ToList();
    }
}