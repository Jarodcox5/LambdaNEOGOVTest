using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NEOGOVLambda
{

    public class Function 
    {
        public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            var k9IdString = request.QueryStringParameters?["K9_Id"];
            var dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            var k9 = await dynamoDbContext.LoadAsync<K9s>(k9IdString);
            
            if (k9 != null)
            {
                return new APIGatewayHttpApiV2ProxyResponse()
                {
                    Body = JsonSerializer.Serialize(k9),
                    StatusCode = 200
                };
            }
            else
            {
                return new APIGatewayHttpApiV2ProxyResponse()
                {
                    Body = "Invalid K9",
                    StatusCode = 404
                }; 
            } 
        }

    } 

    public class K9s
    {
        public string K9_Id { get; set; }
        public string HandlerName { get; set; }
        public string Name { get; set; }
    }
    // request.PathParameters.TryGetValue("k9Id", out var k9IdString);
    // var dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
    // var k9 = await dynamoDbContext.LoadAsync<K9s>(k9IdString);
    //
    // if (k9 != null)
    // {
    //     return new APIGatewayHttpApiV2ProxyResponse()
    //     {
    //         Body = JsonSerializer.Serialize(k9),
    //         StatusCode = 200
    //     };
    // }
    // else
    // {
    //     return new APIGatewayHttpApiV2ProxyResponse()
    //     {
    //         Body = "Invalid K9",
    //         StatusCode = 404
    //     }; 
    // }
    
    // var dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
    // return await dynamoDbContext.LoadAsync<K9s>(k9iId);
    
}
