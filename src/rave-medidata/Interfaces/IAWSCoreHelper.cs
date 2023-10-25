using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theradex.Rave.Medidata.Interfaces
{
    public interface IAWSCoreHelper
    {
        public Task<bool> UploadDataAsync(string bucketName, string objectName, string content);

        Task<List<S3Object>> ListBucketContentsV2Async(string bucketName, string prefix);

        Task<string> DownloadObjectFromBucketAsync(string bucketName, string objectKey);

        Task<bool> UploadStreamAsync(string bucketName, string objectKey, Stream stream);

        //Task<bool> SendEmailAsync(string fromAddress, List<string> toAddresses, string subject, string body);

        Task<bool> SendEmailRawAsync(string fromAddress, List<string> toAddresses, string subject, string body);

        Task<bool> SendEmailRawWithAttachmentAsync(string bucketName, List<string> objectKeys, string fromAddress, List<string> toAddresses, string subject, string body);
    }
}
