using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Amazon.DynamoDBv2.DataModel;

namespace Theradex.ODS.Models
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
        public string ExtractedFileName { get; set; }
        public int TotalPages { get; set; }
        public long PageSize { get; set; }
        public int PageNumber { get; set; }

        public BatchRunControl()
        {
        }
    }
}