using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theradex.ODS.DataAccess.Models
{
    public class BatchRunControl
    {
        public int Id { get; set; }

        public string TableName { get; set; }

        public string ApiStartDate { get; set; }

        public string ApiEndDate { get; set; }

        public string Slot { get; set; }

        public int NoOfRecords { get; set; }

        public string UrlToPullData { get; set; }

        public string RaveUsername { get; set; }

        public string RavePassword { get; set; }

        public string IsRunCompleteFlag { get; set; }

        public DateTime? JobStartTime { get; set; }
        public DateTime? JobEndTime { get; set; }

        public string UrlUsedToGetInterval { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

        public string ErrorMessage { get; set; }
        public int NoOfRecordsRetrieved { get; set; }

        public string RaveDataUrl { get; set; }

        public string HttpStatusCode { get; set; }
        public string Success { get; set; }

        public int NoOfRetry { get; set; } = 0;
        public DateTime? NextRetryTime { get; set; }

        public string Payload { get; set; }
        public Payloads Payloads { get; set; }

        public BatchRunControl()
        {
        }
    }
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