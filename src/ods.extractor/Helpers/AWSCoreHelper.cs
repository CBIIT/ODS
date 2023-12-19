using Amazon;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Theradex.ODS.Extractor.Interfaces;
using Theradex.ODS.Extractor.Models.Configuration;

namespace Theradex.ODS.Extractor.Helpers
{
    public class AWSCoreHelper : IAWSCoreHelper
    {
        private readonly ILogger<AWSCoreHelper> _logger;
        private readonly AppSettings _appSettings;
        private readonly IAmazonS3 _s3Client;
        public AWSCoreHelper(ILogger<AWSCoreHelper> logger, IAmazonS3 s3Client, IOptions<AppSettings> appOptions)
        {
            _logger = logger;
            _appSettings = appOptions.Value;
            _s3Client = s3Client;
        }

        public async Task<bool> CreateBucketAsync(string bucketName)
        {
            try
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                };

                var response = await _s3Client.PutBucketAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId};Successfully created bucket ({bucketName}).");

                    return true;
                }
                else
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId};Could not create bucket ({bucketName}).");

                    return false;
                }
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId};Error creating bucket ({bucketName}): {ex}");

                return false;
            }
        }

        public async Task<bool> DeleteObjectFromBucketAsync(string bucketName, string objectKey)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Deleting object ({objectKey}) from bucket ({bucketName}).");

                var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Object ({objectKey}) deleted from bucket ({bucketName}) successfully.");

                    return true;
                }
                else
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Could not delete ({objectKey}) from bucket ({bucketName}); ErrorCode: {response.HttpStatusCode}");

                    return false;
                }

            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Error deleting Object ({objectKey}) from bucket ({bucketName}): {ex}");

                return false;
            }
        }

        public async Task<ListBucketsResponse> GetAllBucketsAsync()
        {
            try
            {
                return await _s3Client.ListBucketsAsync();
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId};Error getting all the buckets: {ex}");

                return null;
            }
        }

        public async Task<bool> IsKeyExistsAsync(string bucketName, string objectKey)
        {
            try
            {
                var response = await _s3Client.GetObjectMetadataAsync(bucketName, objectKey);

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Object {objectKey} not exists in the bucket {bucketName}; ErrorCode: {ex.ErrorCode}");

                return false;
            }
        }

        public async Task<bool> UploadDataAsync(string bucketName, string objectKey, string content)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    ContentBody = content                    
                };

                var response = await _s3Client.PutObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Successfully uploaded Object ({objectKey}) to bucket ({bucketName}).");

                    return true;
                }
                else
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId}; Could not upload Object ({objectKey}) to bucket ({bucketName}).");

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; Error uploading the object ({objectKey}) to bucket ({bucketName}): {ex}");

                return false;
            }
        }

        public async Task<List<S3Object>> ListBucketContentsAsync(string bucketName)
        {
            try
            {
                var result = new List<S3Object>();

                string token = null;

                do
                {
                    ListObjectsRequest request = new ListObjectsRequest()
                    {
                        BucketName = bucketName
                    };

                    ListObjectsResponse response = await _s3Client.ListObjectsAsync(request).ConfigureAwait(false);

                    result.AddRange(response.S3Objects);

                    token = response.NextMarker;

                } while (token != null);

                return result;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId};Error getting all the objects from the bucket ({bucketName}): {ex}");

                return null;
            }
        }

        public async Task<List<S3Object>> ListBucketContentsV2Async(string bucketName, string prefix)
        {
            try
            {
                var result = new List<S3Object>();

                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = prefix,
                    MaxKeys = 5,
                };

                ListObjectsV2Response response;

                do
                {
                    response = await _s3Client.ListObjectsV2Async(request);

                    result.AddRange(response.S3Objects);

                    // If the response is truncated, set the request ContinuationToken
                    // from the NextContinuationToken property of the response.
                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (response.IsTruncated);

                return result;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId};Error getting all the objects from the bucket ({bucketName}): {ex}");

                return null;
            }
        }

        public async Task<string> DownloadObjectFromBucketAsync(string bucketName, string objectKey)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                using GetObjectResponse response = await _s3Client.GetObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId};Successfully downloaded the Object ({objectKey}) from bucket ({bucketName}).");

                    StreamReader reader = new StreamReader(response.ResponseStream);

                    string content = reader.ReadToEnd();

                    return content;
                }
                else
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId};Could not downloaded the Object ({objectKey}) from bucket ({bucketName}).");

                    return null;
                }
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId};Error downloading the Object ({objectKey}) from bucket ({bucketName}): {ex}.");

                return null;
            }
        }

        public async Task<bool> UploadStreamAsync(string bucketName, string objectKey, Stream stream)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = stream
                };

                var response = await _s3Client.PutObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId};Successfully uploaded Object ({objectKey}) to bucket ({bucketName}).");

                    return true;
                }
                else
                {
                    _logger.LogInformation($"TraceId:{_appSettings.TraceId};Could not upload Object ({objectKey}) to bucket ({bucketName}).");

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId};Error uploading the object ({objectKey}) to bucket ({bucketName}): {ex}");

                return false;
            }
        }

        public async Task<bool> SendEmailRawWithAttachmentAsync(string bucketName, List<string> objectKeys, string fromAddress, List<string> toAddresses, string subject, string body)
        {
            if (objectKeys == null || objectKeys.Count == 0)
                return await SendEmailRawAsync(fromAddress, toAddresses, subject, body);

            try
            {
                using (MemoryStream zipStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                    {
                        foreach (var objectKey in objectKeys)
                        {
                            var entry = archive.CreateEntry(Path.GetFileName(objectKey), CompressionLevel.Fastest);

                            using (var entryStream = entry.Open())
                            {
                                var request = new GetObjectRequest { BucketName = bucketName, Key = objectKey };

                                using (var getObjectResponse = await _s3Client.GetObjectAsync(request))
                                {
                                    await getObjectResponse.ResponseStream.CopyToAsync(entryStream);
                                }
                            }
                        }
                    }

                    using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
                    using (var messageStream = new MemoryStream())
                    {
                        var message = new MimeMessage();
                        var builder = new BodyBuilder() { HtmlBody = body };

                        message.From.Add(new MailboxAddress("IntegrationStatus", fromAddress));

                        foreach (var address in toAddresses)
                            message.To.Add(new MailboxAddress("user", address));

                        message.Subject = subject;

                        MemoryStream attachmentStream = new MemoryStream(zipStream.ToArray());

                        builder.Attachments.Add("IntegrationStatus.zip", attachmentStream);

                        message.Body = builder.ToMessageBody();

                        message.WriteTo(messageStream);

                        var request = new SendRawEmailRequest()
                        {
                            RawMessage = new RawMessage() { Data = messageStream }
                        };

                        await client.SendRawEmailAsync(request);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; SendEmailWithAttachmentAsync failed; Exception: {ex}");

                return false;
            }
        }

        public async Task<bool> SendEmailRawAsync(string fromAddress, List<string> toAddresses, string subject, string body)
        {
            try
            {
                using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
                using (var messageStream = new MemoryStream())
                {
                    var message = new MimeMessage();
                    var builder = new BodyBuilder() { HtmlBody = body };

                    message.From.Add(new MailboxAddress("IntegrationStatus", fromAddress));

                    foreach (var address in toAddresses)
                        message.To.Add(new MailboxAddress("user", address));

                    message.Subject = subject;

                    message.Body = builder.ToMessageBody();

                    message.WriteTo(messageStream);

                    var request = new SendRawEmailRequest()
                    {
                        RawMessage = new RawMessage() { Data = messageStream }
                    };

                    await client.SendRawEmailAsync(request);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceId:{_appSettings.TraceId}; SendEmailRawAsync failed; Exception: {ex}");

                throw;
            }
        }

        //public async Task<bool> SendEmailAsync(string fromAddress, List<string> toAddresses, string subject, string body)
        //{
        //    using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
        //    {
        //        var sendRequest = new SendEmailRequest
        //        {
        //            Source = fromAddress,
        //            Destination = new Destination
        //            {
        //                ToAddresses = toAddresses
        //            },
        //            Message = new Message
        //            {
        //                Subject = new Content(subject),
        //                Body = new Body
        //                {
        //                    Html = new Content
        //                    {
        //                        Charset = "UTF-8",
        //                        Data = body
        //                    }
        //                }
        //            }
        //        };
        //        try
        //        {
        //            var response = await client.SendEmailAsync(sendRequest);

        //            var messageId = response.MessageId;

        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"TraceId:{_appSettings.TraceId}; SendEmailAsync failed; Exception: {ex}");

        //            throw;
        //        }
        //    }
        //}
    }
}