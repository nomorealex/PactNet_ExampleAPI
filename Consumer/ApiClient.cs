using Newtonsoft.Json;
using System.Text;

namespace Consumer.Api
{
    public class ApiClient
    {

        private readonly Uri _uri;

        public ApiClient(Uri uri)
        {
            _uri = uri;
        }

        public async Task<HttpResponseMessage> GetAllOrders()
        {

            using (var client = new HttpClient { BaseAddress = _uri })
            {

                try
                {
                    var response = await client.GetAsync($"/Orders");
                    return response;
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging purposes
                    Console.WriteLine("Exception occurred: " + ex);
                    // Create a response message to indicate an error occurred
                    HttpResponseMessage errorResponse = new HttpResponseMessage();
                    errorResponse.Content = new StringContent("There was a problem: " + ex.Message, Encoding.UTF8, "text/plain");
                    return errorResponse;
                }

            }

        }

        public async Task<HttpResponseMessage> GetOrder(string id)
        {
            using (var client = new HttpClient { BaseAddress = _uri })
            {
                try
                {
                    var response = await client.GetAsync($"/Orders/{id}");
                    return response;
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging purposes
                    Console.WriteLine("Exception occurred: " + ex);
                    // Create a response message to indicate an error occurred
                    HttpResponseMessage errorResponse = new HttpResponseMessage();
                    errorResponse.Content = new StringContent("There was a problem: " + ex.Message, Encoding.UTF8, "text/plain");
                    return errorResponse;
                    //throw new Exception("There was a problem connecting.", ex);
                }
            }
        }

        public async Task<HttpResponseMessage> PostOrder(string? name, string? description)
        {
            using (var client = new HttpClient { BaseAddress = _uri })
            {

                try
                {
                    var orderJson = (name==null || description==null) ? JsonConvert.SerializeObject(new { }) :
                        JsonConvert.SerializeObject(new { Name = name, Description = description }); ;

                    Console.WriteLine($"{orderJson}");
                    var stringContent = new StringContent(orderJson, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("/Orders", stringContent);
                    return response;
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging purposes
                    Console.WriteLine("Exception occurred: " + ex);
                    // Create a response message to indicate an error occurred
                    HttpResponseMessage errorResponse = new HttpResponseMessage();
                    errorResponse.Content = new StringContent("There was a problem: " + ex.Message, Encoding.UTF8, "text/plain");
                    return errorResponse;
                }
            }
        }

        public async Task<HttpResponseMessage> DeleteOrder(string id)
        {
            using (var client = new HttpClient { BaseAddress = _uri })
            {
                try
                {
                    var response = await client.DeleteAsync($"/Orders/{id}");
                    return response;
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging purposes
                    Console.WriteLine("Exception occurred: " + ex);
                    // Create a response message to indicate an error occurred
                    HttpResponseMessage errorResponse = new HttpResponseMessage();
                    errorResponse.Content = new StringContent("There was a problem: " + ex.Message, Encoding.UTF8, "text/plain");
                    return errorResponse;
                }
            }
        }


    }
}