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

        public async Task<PutObjectResponse> PutObject(string key, Stream input)
        {
            var request = new PutObjectRequest {
                BucketName = this.Bucket,
                Key = key,
                InputStream = input
            };
            return await this.Client.PutObjectAsync(request);
        }
    }
}