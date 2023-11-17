using Consumer.Api;
using Newtonsoft.Json;
using PactNet;
using PactNet.Matchers;
using PactNet.Output.Xunit;
using System;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace ConsumerTests
{
    public class ConsumerTests
    {
        private IPactBuilderV3 pact;
        private ApiClient apiClient;
        private readonly int port = 9876;

        //Data expected
        List<dynamic> orders = new List<dynamic>()
            {
                new { Id = 0, Name = "first", Description = "brief_description1" },
                new { Id = 1, Name = "second", Description = "brief_description2"}
            };

        dynamic postOrder = new
        {
            Name = "third",
            Description = "brief_description3"
        };

        private readonly string DeleteId = "1";
        private readonly string GetId = "0";
        private readonly string GetIdDoesNotExist = "5656";


        public ConsumerTests(ITestOutputHelper output)
        {
            var config = new PactConfig
            {
                PactDir = Path.Join("..","..","..","..", "pacts"),
                Outputters = new[] { new XunitOutput(output) },
                LogLevel = PactLogLevel.Debug
            };

            pact = Pact.V3("Consumer", "Provider", config).WithHttpInteractions(port);
            
            apiClient = new ApiClient(new Uri($"http://localhost:{port}"));
        }

        //==========================================GET_Method=====================================

        [Fact]
        public async Task ItHandlesGetOrdersRequest()
        {
            //Arrange
            pact.UponReceiving("A valid GET request to retrieve all orders.")
                    .Given("There is data")
                    .WithRequest(HttpMethod.Get, "/Orders")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(orders));
            //Act
            await pact.VerifyAsync(async ctx =>
            {
                var response = await apiClient.GetAllOrders();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //Assert.Contains(orders.ToString(),response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                string responseBodyAsString = await response.Content.ReadAsStringAsync();
                dynamic responseContent = JsonConvert.DeserializeObject<dynamic>(responseBodyAsString)!;
                Assert.Equal(orders[0].Id,responseContent[0].Id.Value);
                Assert.Equal(orders[0].Name,responseContent[0].Name.ToString());
                Assert.Equal(orders[0].Description,responseContent[0].Description.ToString());
                Assert.Equal(orders[1].Id,responseContent[1].Id.Value);
                Assert.Equal(orders[1].Name,responseContent[1].Name.ToString());
                Assert.Equal(orders[1].Description,responseContent[1].Description.ToString());
                
            });

        }



        [Fact]
        public async Task ItHandlesGetOrderRequest()
        {
            //Arrange
            pact.UponReceiving("A valid GET request to retrieve a specific order.")
                    .Given("There is data")
                    .WithRequest(HttpMethod.Get, $"/Orders/{GetId}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(orders[0]));

            //Act
            await pact.VerifyAsync(async ctx => {
                var response = await apiClient.GetOrder(GetId);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseBodyAsString = await response.Content.ReadAsStringAsync();
                dynamic responseContent = JsonConvert.DeserializeObject<dynamic>(responseBodyAsString)!;
                Assert.Equal(orders[0].Id, responseContent.Id.Value);
                Assert.Equal(orders[0].Name, responseContent.Name.ToString());
                Assert.Equal(orders[0].Description, responseContent.Description.ToString());
            });
        }

        [Fact]
        public async Task ItHandlesGetOrderRequestAndOrderDoesNotExist()
        {
            //Arrange
            pact.UponReceiving("A invalid GET request to retrieve a specific order, id does not exist.")
                    .Given("There is no data")
                    .WithRequest(HttpMethod.Get, $"/Orders/{GetIdDoesNotExist}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.NotFound);

            //Act
            await pact.VerifyAsync(async ctx => {
                var response = await apiClient.GetOrder(GetIdDoesNotExist);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            });
        }

        [Fact]
        public async Task ItHandlesGetOrderRequestAndIdOrderIsInvalid()
        {
            //Arrange
            pact.UponReceiving("A invalid GET request to retrieve a specific order, id differs from integer.")
                .Given("There is no data")
                .WithRequest(HttpMethod.Get, "/Orders/invalidID")
            .WillRespond()
                .WithStatus(HttpStatusCode.BadRequest);

            //Act
            await pact.VerifyAsync(async ctx => {
                var response = await apiClient.GetOrder("invalidID");
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            });
        }



        //==========================================POST_Method=====================================
        

        [Fact]
        public async Task ItHandlesPostOrderRequestAndIdOrderIsValid()
        {
            //Arrange
            pact.UponReceiving("A valid POST request to create a new order.")
                .Given("There is data")
                .WithRequest(HttpMethod.Post, "/Orders")
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new
                {
                    Name = "third",
                    Description = "brief_description3"
                })
            .WillRespond()
                .WithStatus(HttpStatusCode.Created)
                .WithJsonBody(new TypeMatcher(postOrder));

            //Act
            await pact.VerifyAsync(async ctx => {
                var response = await apiClient.PostOrder("third", "brief_description3");
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            });
        }

        [Fact]
        public async Task ItHandlesPostOrderRequestAndWithBodyMissing()
        {
            //Arrange
            pact.UponReceiving("A invalid POST request to create a new order.")
                .Given("There is no data")
                .WithRequest(HttpMethod.Post, "/Orders")
                .WithJsonBody(new TypeMatcher(new
                {
                }))
            .WillRespond()
                .WithStatus(HttpStatusCode.BadRequest);

            //Act
            await pact.VerifyAsync(async ctx =>
            {
                var responseWithMissingBody = await apiClient.PostOrder(null, null);
                Assert.Equal(HttpStatusCode.BadRequest, responseWithMissingBody.StatusCode);
            });
        }



        //==========================================DELETE_Method=====================================
        
        [Fact]
        public async Task ItHandlesDeleteOrderRequestAndOrderExists()
        {
            pact.UponReceiving("A valid DELETE request to delete a specific joke.")
                .Given("There is data")
                .WithRequest(HttpMethod.Delete, $"/Orders/{DeleteId}")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new TypeMatcher(orders[1]));

            await pact.VerifyAsync(async ctx => {
                var response = await apiClient.DeleteOrder(DeleteId);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseBodyAsString = await response.Content.ReadAsStringAsync();
                dynamic responseContent = JsonConvert.DeserializeObject<dynamic>(responseBodyAsString)!;
                Console.WriteLine(responseContent);
                Assert.Equal(orders[1].Id,responseContent.Id.Value);
                Assert.Equal(orders[1].Name,responseContent.Name.ToString());
                Assert.Equal(orders[1].Description,responseContent.Description.ToString());
            });
        }

        [Fact]
        public async Task ItHandlesDeleteOrderRequestAndOrderDoesNotExists()
        {
            pact.UponReceiving("A invalid DELETE request to delete a specific joke, id does not exist.")
                .Given("There is no data")
                .WithRequest(HttpMethod.Delete, $"/Orders/{GetIdDoesNotExist}")
            .WillRespond()
                .WithStatus(HttpStatusCode.NotFound);


            await pact.VerifyAsync(async ctx => {
                var response = await apiClient.DeleteOrder(GetIdDoesNotExist);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            });
        }

        [Fact]
        public async Task ItHandlesDeleteOrderRequestAndIdOrderIsInvalid()
        {
            pact.UponReceiving("A invalid DELETE request to delete a specific joke, id differs from integer.")
                .Given("There is no data")
                .WithRequest(HttpMethod.Delete, "/Orders/invalidID")
            .WillRespond()
                .WithStatus(HttpStatusCode.BadRequest);

            await pact.VerifyAsync(async ctx => {
                var response = await apiClient.DeleteOrder("invalidID");
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            });
        }

    }
}