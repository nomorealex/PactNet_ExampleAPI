using Microsoft.AspNetCore.Mvc;
namespace Provider.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {

        /// <summary>
        /// List representing the repository (storage of the orders for testing)
        /// </summary>
        List<Orders> ordersStorage = new List<Orders>
        {
            new Orders (0,"first","brief_description1" ),
            new Orders (1,"second","brief_description2" ),
        };

        private readonly ILogger<OrdersController> _logger;

        private IConfiguration _configuration { get; }

        public OrdersController(IConfiguration configuration, ILogger<OrdersController> logger)
        {
            this._configuration = configuration;
            _logger = logger;
        }



        /// <summary>
        /// Return all the orders that exist in the list
        /// </summary>
        /// <returns>A list of existing orders</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /Orders
        /// [Body Received]
        /// [
        ///     {
        ///        "Id": 0,
        ///        "Name": "first",
        ///        "Description": "brief_description1"
        ///     },
        ///     {
        ///        "Id": 1,
        ///        "Name": "second",
        ///        "Description": "brief_description2"
        ///     }
        /// ]
        ///
        /// </remarks>
        /// <response code="200">OK. Returns the list of the orders</response>
        /// <response code="204">No Content. If the list is empty</response>
        [HttpGet(Name = "GetOrders")]
        [ProducesResponseType(typeof(List<Orders>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string),StatusCodes.Status204NoContent)]
        public IActionResult GetOrder()
        {
            var allOrders = ordersStorage.ToArray();

            if (allOrders.Count() > 0)
                return Ok(allOrders);
            else
                return NoContent();
        }



        /// <summary>
        /// Return a specific order, specified by Id.
        /// </summary>
        /// <returns>A specific order </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Orders/{id}  [id = 1]
        ///     [Body Received]
        ///     {
        ///        "Id": 1,
        ///        "Name": "second",
        ///        "Description": "brief_description2"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">OK. Returns the list of the orders</response>
        /// <response code="400">BadRequest. If the id provided cannot be parsed to a integer</response>
        /// <response code="404">NotFound. If the id cannot be found in the list of orders</response>
        [HttpGet("{id}", Name = "GetOrderById")]
        [ProducesResponseType(typeof(Orders),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
        public IActionResult GetOrder(string id) 
        {

            int _id = 0;

            if (!(int.TryParse(id, out _id)))
            {
                return BadRequest("BadRequest!");
            }

            Orders order = ordersStorage.FirstOrDefault(j => j.Id == _id);

            if (order != null)
            {
                return Ok(order);
            }

            return NotFound("Order with the id provided not found!");
           
        }



        /// <summary>
        /// Submit a new order to the list.
        /// </summary>
        /// <returns>The complete order (id automatically generated, name and description provided) </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Orders
        ///     [Body Sent]
        ///     {
        ///        "Name": "third",
        ///        "Description": "brief_description3"
        ///     }
        ///     [Body Received]
        ///     {
        ///         "Id" : 2,
        ///         "Name": "third",
        ///         "Description": "brief_description3"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Created. Return the complete order submitted</response>
        /// <response code="400">BadRequest. If a malformed json body was sented (missing 'Name' or 'Description' attribute or both attributes)</response>
        [HttpPost(Name="PostNewOrder")]
        [ProducesResponseType(typeof(Orders),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string),StatusCodes.Status400BadRequest)]
        public IActionResult PostOrder([FromBody] Orders order)
        {

            if (order is Orders _order && order.Id == 0 && order.Name != null && order.Description != null)
            {
                var lastid = GetLastOrdersId();
                _order.Id = (lastid != -1) ? ++lastid : 0;
                ordersStorage.Add(_order);
                return Created($"/Orders/{_order.Id}", _order);
            }

            return BadRequest("BadRequest!");
        }



        /// <summary>
        /// Delele a specific order from the list.
        /// </summary>
        /// <returns>The order that was deleted (id, name and description) </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /Orders/{id}  [id = 1]
        ///     [Body Received]
        ///     {
        ///        "Id" : "1"
        ///        "Name": "second",
        ///        "Description": "brief_description2"
        ///     }
        ///
        /// </remarks>
        /// <response code="202">Accepted. Return the complete order submitted</response>
        /// <response code="400">BadRequest. If a malformed json body was sented (missing 'Name' or 'Description' attribute or both)</response>
        /// <response code="404">NotFound. If the id cannot be found in the list of orders</response>
        [HttpDelete("{id}", Name="DeleteOrderById")]
        [ProducesResponseType(typeof(Orders),StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(string),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
        public IActionResult DeleteOrder(string id)
        {
            int _id = 0;

            if (!(int.TryParse(id, out _id)))
            {
                return BadRequest("BadRequest!");
            }

            Orders order = ordersStorage.FirstOrDefault(j => j.Id == _id);

            if (order != null)
            {
                return Accepted(order);
            }
            return NotFound("Order with the id provided not found!");
        }


        int GetLastOrdersId()
        {
            if (ordersStorage.Count > 0)
            {
                Orders lastOrder = ordersStorage[ordersStorage.Count - 1];
                return lastOrder.Id;
            }
            else
            {
                return -1;
            }
        }

    }
}