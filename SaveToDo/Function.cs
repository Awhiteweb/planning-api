using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.S3;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer( typeof( Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer ) )]

namespace CouttsWhite.SaveToDo
{
    public class Function
    {
        private readonly AmazonS3Client Client;

        public Function()
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
            return await new App( this.Client ).Run( input );
        }
    }

    public class App
    {
        private readonly AmazonS3Client Client;

        public App( AmazonS3Client client )
        {
            this.Client = client;
        }

        public async Task<Stream> Run( string key )
        {
            return await new S3Client( this.Client ).GetObject( key );
        }
    }
}
