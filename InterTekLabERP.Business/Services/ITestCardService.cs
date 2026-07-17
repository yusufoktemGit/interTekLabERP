using interTekLabERP.Entities;

namespace interTekLabERP.Business.Services;

public interface ITestCardService
{
    List<TestCard> GetActive();
}
