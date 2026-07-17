using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterTekLabERP.Entities.Domain
{
    public class SampleLabelDto
    {
        public int Id { get; set; }

        public string SampleCode { get; set; } = string.Empty;

        public string TrackingNo { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public string QrImagePath { get; set; } = string.Empty;
    }
}