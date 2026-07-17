using interTekLabERP.Data;
using interTekLabERP.Entities;

namespace interTekLabERP.Business.Services;

public class TestCardService : ITestCardService
{
    private readonly ApplicationDbContext _context;

    public TestCardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<TestCard> GetActive()
    {
        return _context.TestCards
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToList();
    }
}
