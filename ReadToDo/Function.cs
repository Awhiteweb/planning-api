using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.S3;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer( typeof( Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer ) )]

namespace CouttsWhite.ReadToDo
{
    public class ReadFunction
    {
        private readonly AmazonS3Client Client;

        public ReadFunction()
        {
            this.Client = new AmazonS3Client();
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Stream> FunctionHandler( string input, ILambdaContext context )
        {
            return await new App( this.Client ).Read( input );
        }
    }

    public class ListFunction
    {
        private readonly AmazonS3Client Client;

        public ListFunction()
        {
            this.Client = new AmazonS3Client();
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ListKeyResponse> FunctionHandler( string input, ILambdaContext context )
        {
            return await new App( this.Client ).List( bool.Parse( input ) );
        }
    }

    public class App
    {
        private readonly S3Client Client;
        private readonly string Prefix = "planning";
        private readonly string Year = DateTime.UtcNow.ToString( "yyyy" );

        public App( AmazonS3Client client )
        {
            this.Client = new S3Client( client );
        }

        public async Task<Stream> Read( string key )
        {
            return await this.Client.GetObject( key );
        }

        public async Task<ListKeyResponse> List(bool completed = false, string continuationToken = null)
        {
            return await this.Client.ListObjects( completed ? $"{this.Prefix}/completed/" : $"{this.Prefix}/in-progress/", continuationToken );
        }
    }

    public class ListKeyResponse
    {
        [JsonPropertyName("key_list")]
        public IEnumerable<string> KeyList { get; set; }

        [JsonPropertyName("continuation_token")]
        public string ContinuationToken { get; set; }
    }
}
