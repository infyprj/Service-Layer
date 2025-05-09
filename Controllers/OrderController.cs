using Infosys.Shop3D.DataAccessLayer;
using Infosys.Shop3D.DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Infosys.Shop3D.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        public Shop3DRepository repository;
        public OrderController(Shop3DRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("user/{userId}")]
        public JsonResult GetOrderHistory(int userId)
        {
            List<Order> orders = new List<Order>();
            try
            {
                orders = repository.GetOrderHistory(userId);
            }
            catch (Exception ex)
            {
                orders = null;
            }
            return Json(orders);
        }

        [HttpPost]
        public JsonResult CreateOrder([FromBody] Order order)
        {
            bool status = false;
            try
            {
                status = repository.CreateOrder(order);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Json(new { Success = status });
        }

        [HttpPost("item")]
        public JsonResult AddOrderItem([FromBody] OrderItem orderItem)
        {
            bool status = false;
            try
            {
                status = repository.AddOrderItem(orderItem);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Json(new { Success = status });
        }
    }
}
