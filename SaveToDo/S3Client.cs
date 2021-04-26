using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;


namespace CouttsWhite.SaveToDo
{
    public class S3Client
    {
        private readonly string Bucket = Environment.GetEnvironmentVariable("Bucket");
        private readonly AmazonS3Client Client;

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
    }
}