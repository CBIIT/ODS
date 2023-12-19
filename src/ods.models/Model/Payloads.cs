using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theradex.ODS.Models
{

    [Serializable()]
    public class PayloadItem
    {
        public string TableName { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int TableRowCount { get; set; }
        public int QueryCountofRows { get; set; }
        public JArray Data { get; set; }
        public string JsonDataChecksum { get; set; }
        public string SQLExecuted { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public long PageSize { get; set; }
    }

    public class Payloads
    {
        public List<PayloadItem> Payload { get; set; }
    }
    // Define a class to represent the JSON structure
    public class IntervalDataItem
    {
        private static string format = "yyyy-MM-dd HH:mm:ss";

        public string Bucket { get; set; }

        private string bucketStart;
        public string BucketStart
        {
            get { return bucketStart; }
            set
            {
                bucketStart = value;
                DateTime.TryParseExact(bucketStart.ToString(), format, null, System.Globalization.DateTimeStyles.None, out DateTime dtBucketStart);
                Start = dtBucketStart; // Parse BucketStart and set Start
            }
        }

        private string bucketEnd;
        public string BucketEnd
        {
            get { return bucketEnd; }
            set
            {
                bucketEnd = value;
                DateTime.TryParseExact(bucketEnd.ToString(), format, null, System.Globalization.DateTimeStyles.None, out DateTime dtBucketEnd);
                End = dtBucketEnd; // Parse BucketStart and set Start
            }
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int Records { get; set; }


    }

    public class TableIntervalPayloadItem
    {
        public List<IntervalDataItem> Data { get; set; }
    }

    public class TableIntervalPayload
    {
        public List<TableIntervalPayloadItem> Payload { get; set; }
    }

}
