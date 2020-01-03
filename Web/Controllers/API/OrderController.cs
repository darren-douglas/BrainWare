using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers
{
    using System.Web.Mvc;
    using Infrastructure;
    using Models;

    public class OrderController : ApiController
    {
        private readonly IOrderService orderService;

        public OrderController()
        {
            orderService = new OrderService();
        }

        [HttpGet]
        public IEnumerable<Order> GetOrders(int id = 1)
        {
            return orderService.GetOrdersForCompany(id);
        }
    }
}
