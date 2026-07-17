using interTekLabERP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterTekLabERP.Entities.Domain
{
    public class SampleHistory
    {
        public int Id { get; set; }

        public int SampleId { get; set; }

        public string ActionType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int UserId { get; set; }

        public User? User { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
