using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NEOGOVLambda
{

    public class Function
    {

        public async Task<K9s> FunctionHandler(string input, ILambdaContext context)
        {
            var dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            var k9 = await dynamoDbContext.LoadAsync<K9s>(input);
            return k9;
        }

    }

    public class K9s
    {
        public string K9_Id { get; set; }
        public string HandlerName { get; set; }
        public string Name { get; set; }
    }
    
    
}
