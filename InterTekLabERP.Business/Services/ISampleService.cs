using interTekLabERP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace interTekLabERP.Business.Services
{
    public interface ISampleService
    {
        List<SampleRequest> GetAll();

        SampleRequest? GetById(int id);

        void Add(SampleRequest sampleRequest);

        void Update(SampleRequest sampleRequest);
        void UpdateStatus(int sampleId, int statusId, int userId, string? cargoCompany);

        string GenerateTrackingNo();

        List<SampleRequest> GetForExport(int? statusId, DateTime? startDate, DateTime? endDate, string? search);

        void AddBulk(string offerNo, string customerName, List<BulkSampleRow> rows, int createdBy);

        void Cancel(int sampleId, int userId, string reason);
        void Delete(int id);
        DateTime CalculateTargetDate(DateTime acceptDate, int workingDays);
    }
}
