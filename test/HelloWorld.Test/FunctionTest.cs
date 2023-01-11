using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using Autofac.Extras.Moq;
using Moq;

namespace NEOGOVLambda.Tests
{
    public class FunctionTest
    {
        private readonly Function _neogovLambda = new Function();
        private readonly Mock<IAmazonDynamoDB> _mockDB = new Mock<IAmazonDynamoDB>();
        private readonly APIGatewayProxyRequest _apiGatewayProxyRequest = new APIGatewayProxyRequest();
        private readonly Mock<APIGatewayProxyRequest> _mock2 = new Mock<APIGatewayProxyRequest>();
        private readonly Mock<Function> _mockFunction = new Mock<Function>();
        private readonly Mock<AmazonDynamoDBClient> _mockFunction2 = new Mock<AmazonDynamoDBClient>();

        [Fact]
        public async Task TestGet_Good()
        {

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDynamoDBContext>().Setup(x => x.LoadAsync<Function.K9s>("123", CancellationToken.None))
                    .ReturnsAsync(GetK9("123"));

                var mockedK9 = GetK9("123");
                APIGatewayProxyResponse apiGatewayProxyResponseExpected = new APIGatewayProxyResponse();
                if (mockedK9 != null)
                {
                    apiGatewayProxyResponseExpected = new APIGatewayProxyResponse()
                    {
                        StatusCode = 200,
                        Body = "K9 Added"
                    }; 
                }
                
                
                var cls = mock.Create<Function>();
                
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("K9_Id", "123");
                
                _apiGatewayProxyRequest.QueryStringParameters = parameters;
                Task<APIGatewayProxyResponse> apiGatewayProxyResponseActual = cls.Get(_apiGatewayProxyRequest);
                
                
                Assert.Equal(apiGatewayProxyResponseActual.Result.StatusCode, apiGatewayProxyResponseExpected.StatusCode);

            }
            

        }
        
        [Fact]
        public async Task TestGet_Invalid()
        {

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDynamoDBContext>().Setup(x => x.LoadAsync<Function.K9s>("", CancellationToken.None))
                    .ReturnsAsync(GetK9(""));

                var mockedK9 = GetK9("");
                APIGatewayProxyResponse apiGatewayProxyResponseExpected = new APIGatewayProxyResponse();
                if (mockedK9 != null)
                {
                    apiGatewayProxyResponseExpected = new APIGatewayProxyResponse()
                    {
                        Body = "Invalid K9",
                        StatusCode = 404
                    }; 
                }
                
                
                var cls = mock.Create<Function>();
                
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("K9_Id", "123");
                
                _apiGatewayProxyRequest.QueryStringParameters = parameters;
                Task<APIGatewayProxyResponse> apiGatewayProxyResponseActual = cls.Get(_apiGatewayProxyRequest);
                
                
                Assert.NotEqual(apiGatewayProxyResponseActual.Result.StatusCode, apiGatewayProxyResponseExpected.StatusCode);

            }
            

        }

        private Function.K9s GetK9(string check)
        {
            var testK9 = new Function.K9s();
            testK9.K9_Id = "123";

            if (check == "")
            {
                return null;
            }
            return testK9;
        }
    }
}