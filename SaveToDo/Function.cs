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
        public async Task<string> FunctionHandler( ToDoList input, ILambdaContext context )
        {
            return await new App( this.Client ).Save( input );
        }
    }

    public class App
    {
        private readonly S3Client Client;
        private readonly string Prefix = "planning";
        private readonly IDictionary<bool, string> Keys = new Dictionary<bool, string>
        {
            { true, "completed" },
            { false, "in-progress" }
        };

        public App( AmazonS3Client client )
        {
            this.Client = new S3Client( client );
        }

        public async Task<string> Save( ToDoList list )
        {
            using MemoryStream ms = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync( ms, list );
            await this.Client.PutObject( $"{this.Prefix}/{this.Keys[list.Completed]}/{list.Title}", ms );
            await this.Client.DeleteObject( $"{this.Prefix}/{this.Keys[!list.Completed]}/{list.Title}" );
            return "Completed";
        }
    }

    public class ToDoList
    {
        [JsonPropertyName( "title" )]
        public string Title { get; set; }

        [JsonPropertyName( "items" )]
        public IEnumerable<ToDoItem> Items { get; set; }

        [JsonPropertyName( "completed" )]
        public bool Completed { get; set; }

        [JsonPropertyName( "updated_by" )]
        public string UpdatedBy { get; set; }
    }

    public class ToDoItem
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }

        [JsonPropertyName("updated_by")]
        public string UpdatedBy { get; set; }
    }
}
