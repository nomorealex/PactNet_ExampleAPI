using Consumer.Api;


namespace Consumer
{
    enum State
    {
        FINALIZE,
        RUNNING
    }

    class Program
    {
        static void Main(string[] args)
        {
            var apiClient = new ApiClient(new Uri("http://localhost:5071/"));
            var option = State.RUNNING;

            while (option != State.FINALIZE)
            {
                Console.WriteLine("-------------------\n[DEV_PRODUCT]\nRequests:\n\t1. GetAllOrders()\n\t2. GetOrder\n\t3. PostOrder\n\t4. DeleteOrder\n\t5. Exit Client!");
                var _in = Console.ReadLine();
                int userInput = -1;

                if (int.TryParse(_in, out userInput))
                {
                    if (userInput >= 1 && userInput <= 5)
                    {
                        switch (userInput)
                        {
                            case 1:
                                Console.WriteLine("**Retrieving Orders List**");
                                var response = apiClient.GetAllOrders().GetAwaiter().GetResult();
                                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                                Console.WriteLine($"Response.Code={response.StatusCode}, Response.Body={responseBody}\n\n");
                                break;
                            case 2:
                                Console.WriteLine("**Retrieving Order**\nWhich id: ");
                                string id = Console.ReadLine();
                                var response1 = apiClient.GetOrder(id).GetAwaiter().GetResult();
                                var responseBody1 = response1.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                                Console.WriteLine($"Response.Code={response1.StatusCode}, Response.Body={responseBody1}\n\n");
                                break;
                            case 3:
                                Console.WriteLine("**Post New Joke**");
                                var response2 = apiClient.PostOrder("somejoke", "somebywho").GetAwaiter().GetResult();
                                var responseBody2 = response2.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                                Console.WriteLine($"Response.Code={response2.StatusCode}, Response.Body={responseBody2}\n\n");
                                break;
                            case 4:
                                Console.WriteLine("**Delete Joke**\nWhich id: ");
                                string id1 = Console.ReadLine();
                                var response3 = apiClient.DeleteOrder(id1).GetAwaiter().GetResult();
                                var responseBody3 = response3.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                                Console.WriteLine($"Response.Code={response3.StatusCode}, Response.Body={responseBody3}\n\n");
                                break;
                            case 5:
                                Console.WriteLine("Closing...");
                                option = State.FINALIZE;
                                break;


                        }


                    }
                    else
                    {
                        Console.WriteLine("Provide an integer >=1 and <=5");
                    }
                }
                else
                {
                    Console.WriteLine("INVALID OPTION, Provided an integer >=1 and <=5");
                }

            }


        }

    }

}