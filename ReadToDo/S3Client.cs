using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CouttsWhite.ReadToDo
{
    public class S3Client
    {
        private readonly string Bucket = Environment.GetEnvironmentVariable("Bucket");
        private readonly AmazonS3Client Client;
        private readonly int MaxKeys = 20;

        public S3Client(AmazonS3Client client) 
        {
            this.Client = client;
        }

        public async Task<Stream> GetObject(string key)
        {
            var request = new GetObjectRequest {
                BucketName = this.Bucket,
                Key = key
            };
            var response = await this.Client.GetObjectAsync(request);
            return response.ResponseStream;
        }

        public async Task<ListKeyResponse> ListObjects(string prefix, string continuationToken = null)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = this.Bucket,
                ContinuationToken = continuationToken,
                MaxKeys = this.MaxKeys,
                Prefix = prefix
            };
            var response = await this.Client.ListObjectsV2Async( request );
            var keyList = response.S3Objects.Select( obj => obj.Key );
            return new ListKeyResponse
            {
                KeyList = keyList,
                ContinuationToken = response.NextContinuationToken
            };
        }
    }
}