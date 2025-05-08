using Infosys.Shop3D.DataAccessLayer;
using Infosys.Shop3D.DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Infosys.Shop3D.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        public Shop3DRepository repository;

        public ProductController(Shop3DRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public JsonResult GetAllProducts()
        {
            List<Product> products = new List<Product>();
            try
            {
                products = repository.GetAllProducts();
            }
            catch (Exception ex)
            {
                products = null;
            }
            return Json(products);
        }
    }
}
