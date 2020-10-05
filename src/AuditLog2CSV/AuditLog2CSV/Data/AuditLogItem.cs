using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace AuditLog2CSV.Data {
    public class AuditLogItem {
        [Name("CreationDate")]
        public string CreationDate { get; set; }
        [Name("UserIds")]
        public string UserIds { get; set; }
        [Name("Operations")]
        public string Operations { get; set; }
        [Name("AuditData")]
        public string AuditData { get; set; }


    }
}
