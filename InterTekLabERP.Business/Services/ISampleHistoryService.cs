using interTekLabERP.Entities;
using InterTekLabERP.Entities.Domain;

namespace interTekLabERP.Business.Services;

public interface ISampleHistoryService
{
    void Add(
        int sampleId,
        string actionType,
        string description,
        int userId);

    List<SampleHistory> GetBySampleId(int sampleId);
}