using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theradex.ODS.Models
{

    [Serializable()]
    public class ODSData
    {
        public string TableName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FileNameWithFullPath { get; set; }
        public string FilePath { get; set; }
        public string URL { get; set; }
        public int RecordCount { get; set; }
        public int NoOfRecords { get; set; }

    }
}