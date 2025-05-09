using Infosys.Shop3D.DataAccessLayer;
using Infosys.Shop3D.DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Infosys.Shop3D.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        public Shop3DRepository repository;
        public UserController(Shop3DRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("register")]
        public JsonResult RegisterUser([FromBody] User user)
        {
            User newUser = null;
            try
            {
                newUser = repository.RegisterUser(
                    user.Email, 
                    user.PasswordHash, 
                    user.FirstName, 
                    user.LastName, 
                    user.PhoneNumber,
                    user.Address, 
                    user.City, 
                    user.State, 
                    user.PostalCode, 
                    user.Country, 
                    user.RoleId
                );
            }
            catch (Exception ex)
            {
                newUser = null;
            }
            return Json(new { User = newUser, Success = newUser != null });
        }

        [HttpPost("login")]
        public JsonResult AuthenticateUser([FromBody] LoginModel model)
        {
            User user = null;
            try
            {
                user = repository.AuthenticateUser(model.Email, model.PasswordHash);
            }
            catch (Exception ex)
            {
                user = null;
            }
            return Json(new { User = user, Success = user != null });
        }

        [HttpPut("update")]
        public JsonResult UpdateUserDetails([FromBody] User user)
        {
            bool status = false;
            try
            {
                status = repository.UpdateUserDetails(user);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Json(new { Success = status });
        }

        [HttpPost("logout")]
        public JsonResult Logout()
        {
            // In a REST API, logout is typically handled client-side
            // by discarding the token or session information
            return Json(new { Success = true, Message = "User logged out successfully" });
        }
    }

    // Helper model for login operations
    public class LoginModel
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
