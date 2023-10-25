using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.ODS.Manager.Helpers
{
    public class AwsStreamWriter : IDisposable
    {
        public static long AwsUploadTimeTicks = 0;
        public static long AwsUploadTotalBytes = 0;
        public static long AwsUploadCount = 0;

        private readonly StreamWriter sw;
        private readonly MemoryStream ms;

        private readonly IAmazonS3 S3Client;
        private readonly string BucketName;
        private readonly string KeyName;

        private readonly string UploadId;
        private readonly List<UploadPartResponse> UploadResponses = new List<UploadPartResponse>();

        private readonly int MaxSize = 5 * 1024 * 1024; // min size in AWS

        private int PartNumber = 1;                     // starts at 1 per AWS

        public AwsStreamWriter(IAmazonS3 s3client, string bucketName, string keyName)
        {
            S3Client = s3client;
            BucketName = bucketName;
            KeyName = keyName;

            ms = new MemoryStream();
            sw = new StreamWriter(ms);

            InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = keyName
            };

            long startTick = DateTime.Now.Ticks;
            InitiateMultipartUploadResponse initResponse = s3client.InitiateMultipartUploadAsync(initiateRequest).Result;
            Interlocked.Add(ref AwsUploadTimeTicks, DateTime.Now.Ticks - startTick);

            UploadId = initResponse.UploadId;
        }

        public void Write(string s)
        {
            if (ms.Position >= MaxSize)
            {
                UploadPart();
            }

            sw.Write(s);            
        }

        public void Dispose()
        {
            Interlocked.Increment(ref AwsUploadCount);

            UploadPart();

            CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest()
            {
                BucketName = BucketName,
                Key = KeyName,
                UploadId = UploadId
            };

            completeRequest.AddPartETags(UploadResponses);

            long startTick = DateTime.Now.Ticks;
            CompleteMultipartUploadResponse completeUploadResponse = S3Client.CompleteMultipartUploadAsync(completeRequest).Result;
            Interlocked.Add(ref AwsUploadTimeTicks, DateTime.Now.Ticks - startTick);

            ms.Dispose();
            sw.Dispose();
        }

        private void UploadPart()
        {
            sw.Flush();

            if (ms.Length == 0)
                return;

            long length = ms.Position;
            ms.Position = 0;

            Interlocked.Add(ref AwsUploadTotalBytes, length);

            UploadPartRequest uploadRequest = new UploadPartRequest()
            {
                BucketName = BucketName,
                Key = KeyName,
                UploadId = UploadId,
                PartNumber = PartNumber++,
                PartSize = length,
                InputStream = ms
            };

            long startTick = DateTime.Now.Ticks;
            UploadResponses.Add(S3Client.UploadPartAsync(uploadRequest).Result);
            Interlocked.Add(ref AwsUploadTimeTicks, DateTime.Now.Ticks - startTick);

            ms.Position = 0;
        }
    }
}