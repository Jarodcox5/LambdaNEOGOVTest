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

        private DynamoDBContext _dynamoDbContext;

        public Function()
        {
            _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return request.HttpMethod switch
            {
                "GET" => await HandleGet(request),
                "POST" => await HandlePost(request)
            };
        }

        public async Task<APIGatewayProxyResponse> HandleGet(APIGatewayProxyRequest request)
        {
            var k9IdString = request.QueryStringParameters?["K9_Id"];
            var k9 = await _dynamoDbContext.LoadAsync<K9s>(k9IdString);

            if (k9 != null)
            {
                return new APIGatewayProxyResponse()
                {
                    Body = JsonSerializer.Serialize(k9),
                    StatusCode = 200
                };
            }
            else
            {
                return new APIGatewayProxyResponse()
                {
                    Body = "Invalid K9",
                    StatusCode = 404
                };
            }
        }

        public async Task<APIGatewayProxyResponse> HandlePost(APIGatewayProxyRequest request)
        {
            var k9 = JsonSerializer.Deserialize<K9s>(request.Body);
            if (k9 != null)
            {
                await _dynamoDbContext.SaveAsync(k9);
                return new APIGatewayProxyResponse()
                {
                    StatusCode = 200,
                    Body = "K9 Added"
                };
            }

            return new APIGatewayProxyResponse()
            {
                Body = "Invalid K9",
                StatusCode = 404
            };
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
}
