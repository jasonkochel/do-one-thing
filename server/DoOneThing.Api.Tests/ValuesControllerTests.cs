using System.IO;
using System.Threading.Tasks;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;


namespace DoOneThing.Api.Tests
{
    public class ValuesControllerTests
    {
        [Fact]
        public async Task TestGet()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = await File.ReadAllTextAsync("./SampleRequests/TasksController-Get.json");
            var request = JsonConvert.DeserializeObject<APIGatewayHttpApiV2ProxyRequest>(requestStr);

            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.Equal("[\"value1\",\"value2\"]", response.Body);
            Assert.True(response.Headers.ContainsKey("Content-Type"));
            Assert.Equal("application/json; charset=utf-8", response.Headers["Content-Type"]);
        }
    }
}
