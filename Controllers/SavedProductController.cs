using Infosys.Shop3D.DataAccessLayer;
using Infosys.Shop3D.DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Infosys.Shop3D.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedProductController : Controller
    {
        public Shop3DRepository repository;
        public SavedProductController(Shop3DRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("user/{userId}")]
        public JsonResult GetSavedProducts(int userId)
        {
            List<Product> products = new List<Product>();
            try
            {
                products = repository.GetSavedProducts(userId);
            }
            catch (Exception ex)
            {
                products = null;
            }
            return Json(products);
        }

        [HttpPost]
        public JsonResult SaveProduct([FromBody] SaveProductModel model)
        {
            bool status = false;
            try
            {
                status = repository.SaveProduct(model.UserId, model.ProductId);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Json(new { Success = status });
        }

        [HttpDelete]
        public JsonResult RemoveSavedProduct([FromBody] SaveProductModel model)
        {
            int result = 0;
            try
            {
                result = repository.RemoveSavedProduct(model.UserId, model.ProductId);
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return Json(new { Success = result > 0, RowsAffected = result });
        }
    }

    // Helper model for save/remove product operations
    public class SaveProductModel
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}
